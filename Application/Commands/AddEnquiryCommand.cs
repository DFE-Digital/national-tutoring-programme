using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;
using Application.Extensions;
using Domain;
using Domain.Enums;
using Domain.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MagicLinkType = Domain.Enums.MagicLinkType;
using TuitionType = Domain.Enums.TuitionType;

namespace Application.Commands;

public record AddEnquiryCommand : IRequest<string>
{
    public EnquiryBuildModel? Data { get; set; } = null!;
}

public class AddEnquiryCommandHandler : IRequestHandler<AddEnquiryCommand, string>
{
    private const string EnquiryNumberOfTpsContactedKey = "number_of_tps_contacted";
    private const string EnquirerViewAllResponsesPageLinkKey = "link_to_enquirer_view_all_responses_page";
    private const string EnquiryTpNameKey = "tp_name";
    private const string EnquiryLadNameKey = "local_area_district";
    private const string EnquiryResponseFormLinkKey = "link_to_tp_response_form";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncrypt _aesEncryption;
    private readonly INotificationsClientService _notificationsClientService;
    private readonly IGenerateReferenceNumber _generateReferenceNumber;
    private readonly ILogger<AddEnquiryCommandHandler> _logger;

    public AddEnquiryCommandHandler(IUnitOfWork unitOfWork, IEncrypt aesEncryption,
        INotificationsClientService notificationsClientService,
        IGenerateReferenceNumber generateReferenceNumber, ILogger<AddEnquiryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _aesEncryption = aesEncryption;
        _notificationsClientService = notificationsClientService;
        _generateReferenceNumber = generateReferenceNumber;
        _logger = logger;
    }

    public async Task<string> Handle(AddEnquiryCommand request, CancellationToken cancellationToken)
    {
        var emptyResult = string.Empty;

        //TODO - deal with error and show a message on UI
        //  Expected errors - no TPs, enquirer email failed with 400 to Gov Notify - where return emptyResult below
        //  Unexpected errors - database issues etc
        //  Errors to TP emails - log error, but don't show error to enquirer?
        if (request.Data == null || request.Data.TuitionPartnersForEnquiry == null || request.Data.TuitionPartnersForEnquiry.Count == 0) return emptyResult;

        var tuitionPartnerEnquiry = request.Data.TuitionPartnersForEnquiry.Results.Select(selectedTuitionPartner =>
            new TuitionPartnerEnquiry() { TuitionPartnerId = selectedTuitionPartner.Id }).ToList();

        var enquirerEmailForTestingPurposes = request.Data?.Email!;

        var getEnquirySubmittedToTpNotificationsRecipients = GetEnquirySubmittedToTpNotificationsRecipients(request,
            request.Data!.TuitionPartnersForEnquiry!.Results, enquirerEmailForTestingPurposes);

        var enquiryRequestMagicLinks = getEnquirySubmittedToTpNotificationsRecipients.Select(recipient => new MagicLink()
        { Token = recipient.Token!, MagicLinkTypeId = (int)MagicLinkType.EnquiryRequest }).ToList();

        var getEnquirySubmittedConfirmationToEnquirerNotificationsRecipient = GetEnquirySubmittedConfirmationToEnquirerNotificationsRecipient(request);

        var enquirerViewAllResponsesMagicLink = new MagicLink()
        {
            Token = getEnquirySubmittedConfirmationToEnquirerNotificationsRecipient.Token!,
            MagicLinkTypeId = (int)MagicLinkType.EnquirerViewAllResponses
        };

        enquiryRequestMagicLinks.Add(enquirerViewAllResponsesMagicLink);

        var keyStageSubjects = request.Data.Subjects?.ParseKeyStageSubjects() ?? Array.Empty<KeyStageSubject>();

        if (!keyStageSubjects.Any())
        {
            _logger.LogError("The {request} Input contains no KeyStage and Subjects.", nameof(AddEnquiryCommand));
            return emptyResult;
        }

        var postCode = request.Data.Postcode;

        if (string.IsNullOrEmpty(postCode))
        {
            _logger.LogError("The {request} Input contains no PostCode.", nameof(AddEnquiryCommand));
            return emptyResult;
        }

        var localAuthorityDistrictName = request.Data.TuitionPartnersForEnquiry.LocalAuthorityDistrictName;

        if (string.IsNullOrEmpty(localAuthorityDistrictName))
        {
            _logger.LogError("The {request} Input contains no LocalAuthorityDistrictName.", nameof(AddEnquiryCommand));
            return emptyResult;
        }

        var tuitionTypeId = GetTuitionTypeId(request.Data.TuitionType);

        var enquiry = new Enquiry()
        {
            Email = request.Data?.Email!,
            TutoringLogistics = request.Data?.TutoringLogistics!,
            SENDRequirements = request.Data?.SENDRequirements ?? null,
            AdditionalInformation = request.Data?.AdditionalInformation ?? null,
            TuitionPartnerEnquiry = tuitionPartnerEnquiry,
            MagicLinks = enquiryRequestMagicLinks,
            SupportReferenceNumber = _generateReferenceNumber.GenerateReferenceNumber(),
            KeyStageSubjectEnquiry = GetKeyStageSubjectsEnquiry(keyStageSubjects),
            PostCode = postCode,
            LocalAuthorityDistrict = localAuthorityDistrictName,
            TuitionTypeId = tuitionTypeId
        };

        var dataSaved = false;
        var loop = 0;

        while (!dataSaved && loop < 10)
        {
            try
            {
                loop++;

                _unitOfWork.EnquiryRepository.AddAsync(enquiry, cancellationToken);

                dataSaved = await _unitOfWork.Complete();

            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null &&
                    (ex.InnerException.Message.Contains("duplicate key") ||
                     ex.InnerException.Message.Contains("unique constraint") ||
                     ex.InnerException.Message.Contains("violates unique constraint")))
                {
                    _logger.LogError("Violation on unique constraint. Support Reference Number: {referenceNumber} Error: {ex}", enquiry.SupportReferenceNumber, ex);

                    enquiry.SupportReferenceNumber = _generateReferenceNumber.GenerateReferenceNumber();

                    _logger.LogInformation("Generating new support reference number: {referenceNumber}", enquiry.SupportReferenceNumber);

                    dataSaved = await _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occurred while trying to save the enquiry. Error: {ex}", ex);
                return emptyResult;
            }
        }

        _logger.LogInformation("Enquiry successfully created with magic links. EnquiryId: {enquiryId}", enquiry.Id);

        getEnquirySubmittedConfirmationToEnquirerNotificationsRecipient.Personalisation.AddDefaultEnquiryPersonalisation(enquiry.SupportReferenceNumber, DateTime.Now, request.Data!.BaseServiceUrl!);
        getEnquirySubmittedToTpNotificationsRecipients.ForEach(x => x.Personalisation.AddDefaultEnquiryPersonalisation(enquiry.SupportReferenceNumber, DateTime.Now, request.Data!.BaseServiceUrl!));
        try
        {
            await _notificationsClientService.SendEmailAsync(
                getEnquirySubmittedConfirmationToEnquirerNotificationsRecipient,
                EmailTemplateType.EnquirySubmittedConfirmationToEnquirer);

            await _notificationsClientService.SendEmailAsync(getEnquirySubmittedToTpNotificationsRecipients,
                EmailTemplateType.EnquirySubmittedToTp);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while sending emails to the Tps and Enquirer. Error: {ex}", ex);
        }


        return dataSaved ? enquiry.SupportReferenceNumber : emptyResult;
    }

    private List<NotificationsRecipientDto> GetEnquirySubmittedToTpNotificationsRecipients(AddEnquiryCommand request,
        IEnumerable<TuitionPartnerResult> recipients, string enquirerEmailForTestingPurposes)
    {
        return (from recipient in recipients
                let generateRandomness
                    = _aesEncryption.GenerateRandomToken()
                let token = _aesEncryption.Encrypt(
                    $"Type={nameof(MagicLinkType.EnquiryRequest)}&TuitionPartnerId={recipient.Id}&Email={request.Data!.Email!}&{generateRandomness}")
                let formLink = $"{request.Data!.BaseServiceUrl}/enquiry/respond/response?token={token}"
                select new NotificationsRecipientDto()
                {
                    Email = recipient.Email,
                    EnquirerEmailForTestingPurposes = enquirerEmailForTestingPurposes,
                    Token = token,
                    Personalisation = GetEnquirySubmittedToTpPersonalisation(recipient.Name, formLink, request!.Data!.TuitionPartnersForEnquiry!.LocalAuthorityDistrictName!),
                    PersonalisationPropertiesToAmalgamate = new List<string>() { EnquiryTpNameKey, EnquiryResponseFormLinkKey }
                }).ToList();
    }

    private static Dictionary<string, dynamic> GetEnquirySubmittedToTpPersonalisation(string tpName, string responseFormLink,
        string ladNameKey)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTpNameKey, tpName },
            { EnquiryResponseFormLinkKey, responseFormLink },
            {EnquiryLadNameKey, ladNameKey }
        };

        return personalisation;
    }

    private NotificationsRecipientDto GetEnquirySubmittedConfirmationToEnquirerNotificationsRecipient(AddEnquiryCommand request)
    {
        var generateRandomness
            = _aesEncryption.GenerateRandomToken();
        var token = _aesEncryption.Encrypt(
            $"Type={nameof(MagicLinkType.EnquirerViewAllResponses)}&Email={request.Data!.Email}&{generateRandomness}");
        var pageLink = $"{request.Data?.BaseServiceUrl}/enquiry/respond/all-enquirer-responses?token={token}";

        var result = new NotificationsRecipientDto()
        {
            Email = request.Data?.Email!,
            EnquirerEmailForTestingPurposes = request.Data?.Email!,
            Token = token,
            Personalisation = GetGetEnquirySubmittedConfirmationToEnquirerPersonalisation(request.Data!.TuitionPartnersForEnquiry!.Results!.Count(), pageLink)
        };
        return result;
    }

    private static Dictionary<string, dynamic> GetGetEnquirySubmittedConfirmationToEnquirerPersonalisation(int numberOfTpsContacted,
        string enquirerViewAllResponsesPageLink)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryNumberOfTpsContactedKey, numberOfTpsContacted.ToString() },
            { EnquirerViewAllResponsesPageLinkKey, enquirerViewAllResponsesPageLink },
        };

        return personalisation;
    }

    private static int? GetTuitionTypeId(TuitionType? tuitionType)
    {
        return tuitionType switch
        {
            null => null,
            TuitionType.InSchool => (int)TuitionType.InSchool,
            TuitionType.Online => (int)TuitionType.Online,
            _ => null
        };
    }

    private static List<KeyStageSubjectEnquiry> GetKeyStageSubjectsEnquiry(IEnumerable<KeyStageSubject> keyStageSubjects)
    {
        var keyStageSubjectEnquiry = new List<KeyStageSubjectEnquiry>();

        foreach (var (keyStageId, subjectId) in keyStageSubjects.ToArray().GetIdsForKeyStageSubjects())
        {
            keyStageSubjectEnquiry.Add(new KeyStageSubjectEnquiry()
            {
                KeyStageId = keyStageId,
                SubjectId = subjectId
            });
        }

        return keyStageSubjectEnquiry;
    }
}