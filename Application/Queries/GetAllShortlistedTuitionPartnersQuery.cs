namespace Application.Queries;

public record GetAllShortlistedTuitionPartnersQuery() : IRequest<IEnumerable<ShortlistedTuitionPartner>>
{

}