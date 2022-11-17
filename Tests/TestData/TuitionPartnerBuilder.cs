﻿using Domain;
using Domain.Constants;
using Infrastructure.Migrations;

namespace Tests.TestData;

public record TuitionPartnerBuilder
{
    private static int Ids = 1;
    public int Id { get; private init; } = Ids++;
    public string SeoName { get; private init; } = $"a-tuition-partner-{Ids}";
    public string Name { get; private init; } = "A Tuition Partner";
    public TuitionPartnerLogo? Logo { get; private init; }
    public string Description { get; private init; } = "A Tuition Partner Description";
    public string Website { get; private init; } = "https://website";
    public string PhoneNumber { get; private init; } = "phonenumber";
    public string EmailAddress { get; private init; } = "tp@example.com";
    public string PostalAddress { get; private set; } = "1 High Street\r\nBeautiful City\rThe County\nPostcode";
    public bool SupportsSen { get; private init; }
    public Dictionary<int, TuitionTypes[]> Districts { get; private init; } = new();
    public SubjectBuilder Subjects { get; private init; } = new SubjectBuilder();

    public static implicit operator TuitionPartner(TuitionPartnerBuilder builder) => new()
    {
        Id = builder.Id,
        SeoUrl = builder.SeoName,
        Name = builder.Name,
        Website = builder.Website,
        Description = builder.Description,
        PhoneNumber = builder.PhoneNumber,
        Email = builder.EmailAddress,
        Address = builder.PostalAddress,
        HasSenProvision = builder.SupportsSen,
        LocalAuthorityDistrictCoverage = builder.DistrictCoverage,
        SubjectCoverage = builder.Subjects.SubjectCoverage,
        Prices = builder.Subjects.Prices,
        Logo = builder.Logo,
    };

    public List<LocalAuthorityDistrictCoverage> DistrictCoverage =>
    Districts.SelectMany(district => district.Value.Select(tuition => new LocalAuthorityDistrictCoverage
    {
        LocalAuthorityDistrictId = district.Key,
        TuitionTypeId = (int)tuition,
    })).ToList();

    internal TuitionPartnerBuilder WithId(int id)
        => new TuitionPartnerBuilder(this) with { Id = id };

    internal TuitionPartnerBuilder WithName(string seoName, string? name = null)
        => new TuitionPartnerBuilder(this) with { Name = name ?? seoName, SeoName = seoName };

    internal TuitionPartnerBuilder WithDescription(string description)
        => new TuitionPartnerBuilder(this) with { Description = description };

    internal TuitionPartnerBuilder WithWebsite(string website)
        => new TuitionPartnerBuilder(this) with { Website = website };

    internal TuitionPartnerBuilder WithPhoneNumber(string phonenumber)
        => new TuitionPartnerBuilder(this) with { PhoneNumber = phonenumber };

    internal TuitionPartnerBuilder WithEmailAddress(string email)
        => new TuitionPartnerBuilder(this) with { EmailAddress = email };

    internal TuitionPartnerBuilder WithLogo(string logo, string extension = ".svg")
        => new TuitionPartnerBuilder(this) with
        {
            Logo = new()
            {
                Logo = logo,
                FileExtension = extension,
            }
        };

    internal TuitionPartnerBuilder TaughtIn(District district, params TuitionTypes[] tuition)
        => new TuitionPartnerBuilder(this) with
        {
            Districts = new Dictionary<int, TuitionTypes[]>(Districts)
            {
                [district.Id] = tuition.Any() ? tuition : new[] { TuitionTypes.InSchool }
            }
        };

    internal TuitionPartnerBuilder WithSubjects(Func<SubjectBuilder, SubjectBuilder> config)
        => new TuitionPartnerBuilder(this) with { Subjects = config(new SubjectBuilder()) };

    internal TuitionPartnerBuilder WithSen(bool supportsSend)
        => new TuitionPartnerBuilder(this) with { SupportsSen = supportsSend };

    internal TuitionPartnerBuilder WithAddress(string address)
        => new TuitionPartnerBuilder(this) with { PostalAddress = address };
}