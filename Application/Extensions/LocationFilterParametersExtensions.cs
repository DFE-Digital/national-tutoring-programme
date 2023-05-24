﻿using Domain;
using Domain.Constants;
using Domain.Search;

namespace Application.Extensions;

public static class LocationFilterParametersExtensions
{
    public static IResult<LocationFilterParameters> TryValidate(this LocationFilterParameters? parameters, bool testIsSchoolPostcode = false)
    {
        if (parameters == null)
        {
            return new LocationNotFoundResult();
        }

        if (parameters.Country != Country.Name.England)
        {
            return new LocationNotAvailableResult();
        }

        if (string.IsNullOrWhiteSpace(parameters.LocalAuthorityDistrictCode))
        {
            return new LocationNotMappedResult();
        }

        if (testIsSchoolPostcode && !parameters.Urn.HasValue)
        {
            return new LocationNotASchool();
        }

        return Result.Success(parameters);
    }
}

public class LocationNotMappedResult : ErrorResult<LocationFilterParameters, string>
{
    public LocationNotMappedResult() : base("Could not identify Local Authority for the supplied postcode")
    {
    }
}

public class LocationNotAvailableResult : ErrorResult<LocationFilterParameters, string>
{
    public LocationNotAvailableResult() : base("This service covers England only")
    {
    }
}

public class LocationNotFoundResult : ErrorResult<LocationFilterParameters, string>
{
    public LocationNotFoundResult() : base("Enter a real postcode")
    {
    }
}

public class LocationNotASchool : ErrorResult<LocationFilterParameters, string>
{
    public LocationNotASchool() : base("Enter a real school postcode")
    {
    }
}