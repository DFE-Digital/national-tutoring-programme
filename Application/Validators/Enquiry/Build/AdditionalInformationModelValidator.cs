﻿using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class AdditionalInformationModelValidator : AbstractValidator<AdditionalInformationModel>
{
    public AdditionalInformationModelValidator()
    {
        RuleFor(request => request.AdditionalInformation)
            .Must(x => string.IsNullOrEmpty(x) || (!string.IsNullOrEmpty(x) && x.Replace(Environment.NewLine, " ").Length <= IntegerConstants.EnquiryQuestionsMaxCharacterSize))
            .WithMessage($"Any other considerations for tuition partners to consider must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize:N0} characters or less");
    }
}