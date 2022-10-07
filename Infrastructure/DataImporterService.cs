﻿using Application.DataImport;
using Application.Factories;
using Application.Mapping;
using Domain;
using Domain.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class DataImporterService : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DataImporterService(IHostApplicationLifetime host, ILogger<DataImporterService> logger, IServiceScopeFactory scopeFactory)
    {
        _host = host;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();
        var dataFileEnumerable = scope.ServiceProvider.GetRequiredService<IDataFileEnumerable>();
        var logoFileEnumerable = scope.ServiceProvider.GetRequiredService<ILogoFileEnumerable>();
        var factory = scope.ServiceProvider.GetRequiredService<ITuitionPartnerFactory>();

        _logger.LogInformation("Migrating database");
        await dbContext.Database.MigrateAsync(cancellationToken);

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                await RemoveTuitionPartners(dbContext, cancellationToken);

                await ImportTuitionPartnerFiles(dbContext, dataFileEnumerable, factory, cancellationToken);

                await ImportTutionPartnerLogos(dbContext, logoFileEnumerable, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            });


        var generalInformatioAboutSchoolsRecords = scope.ServiceProvider.GetRequiredService<IGeneralInformationAboutSchoolsRecords>();
        var giasFactory = scope.ServiceProvider.GetRequiredService<IGeneralInformationAboutSchoolsFactory>();

        await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                await RemoveGeneralInformationAboutSchools(dbContext, cancellationToken);

                await ImportGeneralInformationAboutSchools(dbContext, generalInformatioAboutSchoolsRecords, giasFactory, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            });

        _host.StopApplication();
    }

    private async Task ImportGeneralInformationAboutSchools(NtpDbContext dbContext, IGeneralInformationAboutSchoolsRecords generalInformatioAboutSchoolsRecords, IGeneralInformationAboutSchoolsFactory giasFactory, CancellationToken cancellationToken)
    {
        var LocalAuthorityDistrictsIds = dbContext.LocalAuthorityDistricts.Select(t => new { t.Code, t.Id })
            .ToDictionary(t => t.Code, t => t.Id);

        var LocalAuthorityIds = dbContext.LocalAuthority.Select(t => new { t.Id, t.Code, })
           .ToDictionary(t => t.Id, t => t.Code);

        var result = generalInformatioAboutSchoolsRecords.GetSchoolDataAsync(cancellationToken);


        foreach (SchoolDatum schoolDatum in result.Result)
        {
            var EstablishmentName = schoolDatum.Name;

            _logger.LogInformation("Attempting to create General Information About Schools from record {EstablishmentName}", EstablishmentName);
            School school;
            try
            {
                school = giasFactory.GetGeneralInformationAboutSchool(schoolDatum, LocalAuthorityDistrictsIds, LocalAuthorityIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown when creating General Information About Schools from record {OriginalFilename}", EstablishmentName);
                continue;
            }

            var validator = new GeneralInformationAboutSchoolValidator();
            var results = await validator.ValidateAsync(school, cancellationToken);
            if (!results.IsValid)
            {
                _logger.LogError($"Establishment name {{TuitionPartnerName}} General Information About Schools created from recoord {{originalFilename}} is not valid.{Environment.NewLine}{{Errors}}",
                    school.EstablishmentName, school.EstablishmentName, string.Join(Environment.NewLine, results.Errors));
                continue;
            }

            dbContext.GeneralInformationAboutSchools.Add(school);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added General Information About School for {EstablishmentName} with id of {id}",
                school.EstablishmentName, school.Id);
        }
    }

    private async Task RemoveTuitionPartners(NtpDbContext dbContext, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting all existing Tuition Partner data");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"LocalAuthorityDistrictCoverage\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"SubjectCoverage\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Prices\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"TuitionPartners\"", cancellationToken: cancellationToken);
    }

    private async Task RemoveGeneralInformationAboutSchools(NtpDbContext dbContext, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting all existing General Information About Schools data");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"GeneralInformationAboutSchools\"", cancellationToken: cancellationToken);
    }

    private async Task ImportTuitionPartnerFiles(NtpDbContext dbContext, IDataFileEnumerable dataFileEnumerable, ITuitionPartnerFactory factory, CancellationToken cancellationToken)
    {
        foreach (var dataFile in dataFileEnumerable)
        {
            var originalFilename = dataFile.Filename;

            _logger.LogInformation("Attempting to create Tuition Partner from file {OriginalFilename}", originalFilename);
            TuitionPartner tuitionPartner;
            try
            {
                tuitionPartner = await factory.GetTuitionPartner(dataFile.Stream.Value, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown when creating Tuition Partner from file {OriginalFilename}", originalFilename);
                continue;
            }

            var validator = new TuitionPartnerValidator();
            var results = await validator.ValidateAsync(tuitionPartner, cancellationToken);
            if (!results.IsValid)
            {
                _logger.LogError($"Tuition Partner name {{TuitionPartnerName}} created from file {{originalFilename}} is not valid.{Environment.NewLine}{{Errors}}",
                    tuitionPartner.Name, originalFilename, string.Join(Environment.NewLine, results.Errors));
                continue;
            }

            dbContext.TuitionPartners.Add(tuitionPartner);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added Tuition Partner {TuitionPartnerName} with id of {TuitionPartnerId} from file {OriginalFilename}",
                tuitionPartner.Name, tuitionPartner.Id, originalFilename);
        }
    }

    private async Task ImportTutionPartnerLogos(NtpDbContext dbContext, ILogoFileEnumerable logoFileEnumerable, CancellationToken cancellationToken)
    {
        var partners = await dbContext.TuitionPartners
            .Select(x => new { x.Id, x.SeoUrl })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Looking for logos for {Count} tuition partners", partners.Count);

        var logos = logoFileEnumerable.ToList();
        _logger.LogInformation("Available logo files are {LogoFiles}",
            string.Join("\n", logos.Select(x => x.Filename).OrderBy(x => x)));

        foreach (var partner in partners)
        {
            var dataFile = logos
                .Where(logo => logo.Filename.Contains(partner.SeoUrl))
                .FirstOrDefault();

            if (dataFile == null)
            {
                _logger.LogInformation("No logo file for Tution Partner {Name}", partner.SeoUrl);
                continue;
            }

            _logger.LogInformation("Retrieving logo file for Tution Partner {Name}", partner.SeoUrl);
            var b64 = Convert.ToBase64String(dataFile.Stream.Value.ReadAllBytes());

            var tp = dbContext.TuitionPartners.Find(partner.Id);
            tp!.Logo = new TuitionPartnerLogo { Logo = b64 };
        }

        await dbContext.SaveChangesAsync();

        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public static class StreamExtensions
{
    public static byte[] ReadAllBytes(this Stream instream)
    {
        if (instream is MemoryStream)
            return ((MemoryStream)instream).ToArray();

        using (var memoryStream = new MemoryStream())
        {
            instream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}