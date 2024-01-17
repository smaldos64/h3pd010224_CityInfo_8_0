using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IRepositoryWrapper
    {
        ICityRepository CityRepositoryWrapper { get; }

        ICityLanguageRepository CityLanguageRepositoryWrapper { get; }

        ILanguageRepository LanguageRepositoryWrapper { get; }

        ICountryRepository CountryRepositoryWrapper { get; }

        IPointOfInterestRepository PointOfInterestRepositoryWrapper { get; }

        int Save();

    }
}
