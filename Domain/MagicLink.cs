namespace Domain;

public class MagicLink
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(14);

    public int? EnquiryId { get; set; }

    public int? MagicLinkTypeId { get; set; }

    public Enquiry? Enquiry { get; set; } = null;

    public MagicLinkType? MagicLinkType { get; set; } = null;
}