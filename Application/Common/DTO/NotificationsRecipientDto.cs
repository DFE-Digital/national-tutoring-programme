namespace Application.Common.DTO;

public class NotificationsRecipientDto
{
    public NotificationsRecipientDto()
    {
        PersonalisationPropertiesToAmalgamate = new List<string>();
    }


    public int TuitionPartnerId { get; set; }

    public string? TuitionPartnerName { get; set; }

    public string OriginalEmail { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string EnquirerEmailForTestingPurposes { get; set; } = null!;

    public string? Token { get; set; }

    public Dictionary<string, dynamic> Personalisation { get; set; } = null!;

    public List<string> PersonalisationPropertiesToAmalgamate { get; set; } = null!;

    public string ClientReference { get; set; } = null!;

    public string ClientReferenceIfAmalgamate { get; set; } = null!;
}