namespace Application.Common.Models.Enquiry.Respond;

public record EnquirerViewResponseModel : EnquiryResponseModel
{
    public string EnquirerViewAllResponsesToken { get; set; } = null!;

}