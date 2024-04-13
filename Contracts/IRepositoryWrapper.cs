using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IRepositoryWrapper
    {
        public DatabaseContext GetCurrentDatabaseContext();

        ICityRepository CityRepositoryWrapper { get; }

        ICityLanguageRepository CityLanguageRepositoryWrapper { get; }

        ILanguageRepository LanguageRepositoryWrapper { get; }

        ICountryRepository CountryRepositoryWrapper { get; }

        IPointOfInterestRepository PointOfInterestRepositoryWrapper { get; }

        Task<int> Save();

    }
}
