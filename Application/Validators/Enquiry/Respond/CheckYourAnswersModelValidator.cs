using Application.Common.Models.Enquiry.Respond;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Respond;

public class CheckYourAnswersModelValidator : AbstractValidator<CheckYourAnswersModel>
{
    public CheckYourAnswersModelValidator()
    {
        RuleFor(request => request.KeyStageAndSubjectsText)
            .NotEmpty()
            .WithMessage("Enter can you support these key stages and subjects")
            .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .WithMessage($"Tuition type must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");

        RuleFor(request => request.TuitionTypeText)
            .NotEmpty()
            .WithMessage("Enter to what extent can you support that type of tuition")
            .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .WithMessage($"Tuition type must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");

        RuleFor(request => request.TutoringLogisticsText)
            .NotEmpty()
            .WithMessage("Enter the type of tuition plan that you need")
            .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .WithMessage($"Tuition plan must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");

        RuleFor(request => request.SENDRequirementsText)
            .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .WithMessage($"SEND requirements must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");

        RuleFor(request => request.AdditionalInformationText)
            .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .WithMessage($"Other tuition considerations must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");
    }
}