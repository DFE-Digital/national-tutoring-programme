﻿using Application.Common.DTO;
using Domain.Enums;

namespace Application.Extensions;

public static class NotificationsRecipientDtoExtensions
{
    public static void AddDefaultEnquiryDetails(this NotificationsRecipientDto notificationsRecipient,
    string enquiryRef, string baseUrl, EmailTemplateType emailTemplateType, DateTime? dateTime, string? tpName = null)
    {
        notificationsRecipient.Personalisation.AddDefaultEnquiryPersonalisation(enquiryRef, baseUrl, dateTime);

        notificationsRecipient.ClientReference = enquiryRef.CreateNotifyClientReference(emailTemplateType, tpName);
        notificationsRecipient.ClientReferenceIfAmalgamate = enquiryRef.CreateNotifyClientReference(emailTemplateType);
    }

    public static void AddDefaultEmailDetails(this NotificationsRecipientDto notificationsRecipient,
        string baseUrl, EmailTemplateType emailTemplateType)
    {
        notificationsRecipient.Personalisation.AddDefaultEnquiryPersonalisation(null, baseUrl, null);

        notificationsRecipient.ClientReference = emailTemplateType.DisplayName();
        notificationsRecipient.ClientReferenceIfAmalgamate = emailTemplateType.DisplayName();
    }
}