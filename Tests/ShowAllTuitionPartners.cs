﻿using Domain.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using UI.Extensions;
using UI.Pages;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class ShowAllTuitionPartners : CleanSliceFixture
{
    public ShowAllTuitionPartners(SliceFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Displays_all_tuition_partners_in_database_in_alphabetical_ordering()
    {
        // Given
        await Fixture.ExecuteDbContextAsync(async db =>
        {
            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Beta",
                SeoUrl = "beta",
                Website = "http://"
            });

            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Gamma",
                SeoUrl = "gamma",
                Website = "http://"
            });

            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Alpha",
                SeoUrl = "alpha",
                Website = "http://"
            });

            await db.SaveChangesAsync();
        });

        // When
        var page = await Fixture.GetPage<AllTuitionPartners>()
            .Execute(async page =>
            {
                await page.OnGet();
                return page;
            });

        // Then
        page.Results.Should().NotBeNull();
        page.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha" },
            new { Name = "Beta" },
            new { Name = "Gamma" }
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Search_by_name()
    {
        // Given
        await Fixture.ExecuteDbContextAsync(async db =>
        {
            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Alpha",
                SeoUrl = "alpha",
                Website = "http://"
            });

            db.TuitionPartners.Add(new Domain.TuitionPartner
            {
                Name = "Beta",
                SeoUrl = "beta",
                Website = "http://"
            });

            await db.SaveChangesAsync();
        });

        // When
        var page = await Fixture.GetPage<AllTuitionPartners>()
            .Execute(async page =>
            {
                page.Data.Name = "LPh";
                await page.OnGet();
                return page;
            });

        // Then
        page.Results.Should().NotBeNull();
        page.Results!.Results.Should().BeEquivalentTo(new[]
        {
            new { Name = "Alpha" }
        });
    }

    [Fact]
    public async Task Sets_from_full_list()
    {
        var page = await Fixture.GetPage<AllTuitionPartners>()
            .Execute(async page =>
            {
                page.Data.From = ReferrerList.SearchResults;
                await page.OnGet();
                return page;
            });

        page.Data.From.Should().Be(ReferrerList.FullList);
    }

    [Fact]
    public async Task Sets_AllSearchData()
    {
        var page = await Fixture.GetPage<AllTuitionPartners>()
            .Execute(async page =>
            {
                page.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
                page.Data.From = ReferrerList.SearchResults;
                page.Data.Name = "test";
                await page.OnGet();
                return page;
            });

        var data = page.TempData.Peek<SearchModel>("AllSearchData");
        data.Should().NotBeNull();
        data!.From.Should().Be(ReferrerList.FullList);
        data.Name.Should().Be("test");
        data.Postcode.Should().BeNull();
        data.Subjects.Should().BeNull();
        data.TuitionType.Should().BeNull();
        data.KeyStages.Should().BeNull();
    }
}