﻿using Application.Repositories;
using Domain.Deltas;

namespace Infrastructure.Repositories;

public class TuitionPartnerRepository : ITuitionPartnerRepository
{
    private readonly NtpDbContext _dbContext;

    public TuitionPartnerRepository(NtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ApplyDeltas(TuitionPartnerDeltas deltas)
    {
        foreach (var toAdd in deltas.Add)
        {
            _dbContext.TuitionPartners.Add(toAdd);
        }

        foreach (var toUpdateDelta in deltas.Update)
        {
            var toUpdate = await _dbContext.TuitionPartners.FindAsync(toUpdateDelta.Id);

            if (toUpdate == null) continue;

            toUpdate.Name = toUpdateDelta.Name;
            toUpdate.Website = toUpdateDelta.Website;

            foreach (var coverageToAdd in toUpdateDelta.CoverageAdd)
            {
                coverageToAdd.TuitionPartner = toUpdate;
                toUpdate.Coverage.Add(coverageToAdd);
            }

            foreach (var coverageToRemoveDelta in toUpdateDelta.CoverageRemove)
            {
                var coverageToRemove = await _dbContext.TuitionPartnerCoverage.FindAsync(coverageToRemoveDelta.Id);
                if (coverageToRemove == null) continue;

                toUpdate.Coverage.Remove(coverageToRemove);
            }
        }

        foreach (var toRemoveDelta in deltas.Remove)
        {
            var toRemove = await _dbContext.TuitionPartners.FindAsync(toRemoveDelta.Id);
            if (toRemove == null) continue;

            _dbContext.TuitionPartners.Remove(toRemove);
        }

        await _dbContext.SaveChangesAsync();

    }
}