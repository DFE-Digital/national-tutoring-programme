using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class EnquirerEmail : PageModel
{
    private readonly ISessionService _sessionService;

    public EnquirerEmail(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    [BindProperty] public EnquirerEmailModel Data { get; set; } = new();

    public async Task<IActionResult> OnGet(EnquirerEmailModel data)
    {
        Data = data;

        var sessionId = Request.Cookies[StringConstants.SessionCookieName];

        if (sessionId == null)
        {
            _sessionService.InitSession();
            return Page();
        }

        var sessionValues = await _sessionService.RetrieveDataAsync(sessionId);

        if (sessionValues == null) return Page();

        //TODO - for the back link to work with correct query string values (see SearchModelExtensions.ToRouteData) then we need do one of the following:
        //  Store all query data in session and extract the session data in to the Data object
        //  Or pass the query string as data (would need hidden imputs on each page) through all the questions in the enquiry
        foreach (var sessionValue in sessionValues.Where(sessionValue => sessionValue.Key.Contains(StringConstants.EnquirerEmail)))
        {
            Data.Email = sessionValue.Value;
        }

        ModelState.Clear();

        return Page();
    }
    public async Task<IActionResult> OnGetSubmit(EnquirerEmailModel data)
    {
        if (ModelState.IsValid)
        {
            var sessionId = Request.Cookies[StringConstants.SessionCookieName];

            if (sessionId != null)
            {
                await _sessionService.AddOrUpdateDataAsync(sessionId, new Dictionary<string, string>()
                {
                    { StringConstants.EnquirerEmail, data.Email!}

                });

                if (!string.IsNullOrEmpty(data.Postcode))
                {
                    await _sessionService.AddOrUpdateDataAsync(sessionId, new Dictionary<string, string>()
                    {
                        { StringConstants.PostCode, data.Postcode },

                    });
                }

                if (data.KeyStages != null)
                {
                    await _sessionService.AddOrUpdateDataAsync(sessionId, new Dictionary<string, string>()
                    {
                        { StringConstants.KeyStages, string.Join(",", data.KeyStages) },
                        { StringConstants.Subjects, string.Join(",", data.Subjects!) },
                    });
                }
            }

            if (data.From == ReferrerList.CheckYourAnswers)
            {
                return RedirectToPage(nameof(CheckYourAnswers));
            }

            return RedirectToPage(nameof(EnquiryQuestion), new SearchModel(data));
        }

        return Page();
    }
}