using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Build;
using Domain;

namespace UI.Pages.Enquiry.Build;

public class SchoolPostcode : PageModel
{
    private readonly ISessionService _sessionService;
    private readonly IMediator _mediator;

    public SchoolPostcode(ISessionService sessionService, IMediator mediator)
    {
        _sessionService = sessionService;
        _mediator = mediator;
    }
    [BindProperty] public SchoolPostcodeModel Data { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(SchoolPostcodeModel data)
    {
        Data = data;

        if (!string.IsNullOrWhiteSpace(Data.Postcode) && string.IsNullOrWhiteSpace(Data.SchoolPostcode))
        {
            var locationResult = await _mediator.Send(new GetSearchLocationQuery(Data.Postcode));
            if (locationResult.TryValidate().IsSuccess)
            {
                Data.SchoolPostcode = Data.Postcode;
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(SchoolPostcodeModel data)
    {
        Data = data;
        if (!ModelState.IsValid) return Page();

        var locationResult = await _mediator.Send(new GetSearchLocationQuery(data.SchoolPostcode!));

        var locationValidationResult = locationResult.TryValidate(true);

        if (locationValidationResult.IsSuccess)
            return RedirectToPage(nameof(ConfirmSchool), data);

        if (locationValidationResult is ErrorResult error)
            ModelState.AddModelError("Data.SchoolPostcode", error.ToString());

        return Page();
    }
}