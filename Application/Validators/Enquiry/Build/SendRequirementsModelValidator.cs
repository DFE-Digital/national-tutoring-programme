﻿using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class SendRequirementsModelValidator : AbstractValidator<SendRequirementsModel>
{
    public SendRequirementsModelValidator()
    {
        RuleFor(request => request.SendRequirements)
             .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
             .WithMessage($"Do you need tuition partners who can support with SEND must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");
    }
}