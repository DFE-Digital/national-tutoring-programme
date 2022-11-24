using Application;
using Domain.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UI.Enums;
using UI.Models;

namespace UI.Pages;

public class AllTuitionPartners : PageModel
{
    private readonly ITuitionPartnerService _tuitionPartnerService;

    public AllTuitionPartners(ITuitionPartnerService tuitionPartnerService) => _tuitionPartnerService = tuitionPartnerService;

    [BindProperty(SupportsGet = true)]
    public SearchModel Data { get; set; } = new();

    public TuitionPartnersResult? Results { get; private set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Data.From = ReferrerList.FullList;

        var tuitionPartners = await FindTuitionPartners(Data, cancellationToken);

        Results = new TuitionPartnersResult(tuitionPartners, tuitionPartners.Count(), null);
    }

    private async Task<IEnumerable<TuitionPartnerResult>> FindTuitionPartners(SearchModel data, CancellationToken cancellationToken)
    {
        int[]? tuitionPartnersIds = null;

        //Only filter if Name is passed in
        if (!string.IsNullOrWhiteSpace(data.Name))
        {
            tuitionPartnersIds = await _tuitionPartnerService.GetTuitionPartnersFilteredAsync(new TuitionPartnersFilter
            {
                Name = data.Name
            }, cancellationToken);
        }

        var tuitionPartners = await _tuitionPartnerService.GetTuitionPartnersAsync(new TuitionPartnerRequest() { TuitionPartnerIds = tuitionPartnersIds }, cancellationToken);

        tuitionPartners = _tuitionPartnerService.OrderTuitionPartners(tuitionPartners);

        return tuitionPartners;
    }
}