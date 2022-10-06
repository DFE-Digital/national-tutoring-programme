﻿using Application.DataImport;
using Application.Factories;
using Domain;
using Domain.Validators;
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

        _host.StopApplication();
    }

    private async Task RemoveTuitionPartners(NtpDbContext dbContext, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting all existing Tuition Partner data");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"LocalAuthorityDistrictCoverage\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"SubjectCoverage\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Prices\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"TuitionPartners\"", cancellationToken: cancellationToken);
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

        var logos = logoFileEnumerable.Where(x => x.Filename.EndsWith(".svg")).ToList();

        var matching = (from p in partners
                        from l in logos
                        where IsFileLogoForTuitionPartner(p.SeoUrl, l.Filename)
                        select new
                        {
                            Partner = p,
                            Logo = l,
                        })
                       .ToList();

        _logger.LogInformation("Matched {Count} logos to tuition partners:\n{Matches}",
            matching.Count, string.Join("\n", matching.Select(x => $"{x.Partner.SeoUrl} => {x.Logo.Filename}")));

        var partnersWithoutLogos = partners.Except(matching.Select(x => x.Partner));
        if (partnersWithoutLogos.Any())
        {
            _logger.LogInformation("{Count} tuition partners do not have logos:\n{WithoutLogo}",
                partnersWithoutLogos.Count(), string.Join("\n", partnersWithoutLogos.Select(x => x.SeoUrl)));
        }

        var logosWithoutPartners = logos.Except(matching.Select(x => x.Logo));
        if (logosWithoutPartners.Any())
        {
            _logger.LogWarning("{Count} logos files do not match a tuition partner:\n{UnmatchedLogos}",
                logosWithoutPartners.Count(), string.Join("\n", logosWithoutPartners.Select(x => x.Filename)));
        }

        foreach (var import in matching)
        {
            _logger.LogInformation("Retrieving logo file for Tution Partner {Name}", import.Partner.SeoUrl);
            var b64 = Convert.ToBase64String(import.Logo.Stream.Value.ReadAllBytes());

            var tp = dbContext.TuitionPartners.Find(import.Partner.Id);
            tp!.Logo = new TuitionPartnerLogo { Logo = b64 };
        }

        await dbContext.SaveChangesAsync();

        await Task.CompletedTask;
    }

    public static bool IsFileLogoForTuitionPartner(string tuitionPartnerName, string logoFilename)
    {
        return logoFilename.Equals($"Logo_{tuitionPartnerName}.svg", StringComparison.InvariantCultureIgnoreCase);
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