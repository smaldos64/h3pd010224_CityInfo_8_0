using Entities;
using Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    // Wrapper funktionaliten i RepositoryWrapper filen her ses også
    // benævnt UnitOfWork i andre sammenhænge.
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private DatabaseContext _repoContext;

        private ICityRepository? _cityRepositoryWrapper;
        private ICityLanguageRepository? _cityLanguageRepositoryWrapper;
        private ILanguageRepository? _languageRepositoryWrapper;
        private ICountryRepository? _countryRepositoryWrapper;
        private IPointOfInterestRepository? _pointOfInterestRepositoryWrapper;

        public RepositoryWrapper(DatabaseContext repositoryContext)
        {
            this._repoContext = repositoryContext;
        }

        public ICityRepository CityRepositoryWrapper
        {
            get
            {
                if (null == _cityRepositoryWrapper)
                {
                    _cityRepositoryWrapper = new CityRepository(_repoContext);
                }

                return (_cityRepositoryWrapper);
            }
        }

        public ICityLanguageRepository CityLanguageRepositoryWrapper
        {
            get
            {
                if (null == _cityLanguageRepositoryWrapper)
                {
                    _cityLanguageRepositoryWrapper = new CityLanguageRepository(_repoContext);
                }

                return (_cityLanguageRepositoryWrapper);
            }
        }

        public ILanguageRepository LanguageRepositoryWrapper
        {
            get
            {
                if (null == _languageRepositoryWrapper)
                {
                    _languageRepositoryWrapper = new LanguageRepository(_repoContext);
                }

                return (_languageRepositoryWrapper);
            }
        }

        public ICountryRepository CountryRepositoryWrapper
        {
            get
            {
                if (null == _countryRepositoryWrapper)
                {
                    _countryRepositoryWrapper = new CountryRepository(_repoContext);
                }

                return (_countryRepositoryWrapper);
            }
        }

        public IPointOfInterestRepository PointOfInterestRepositoryWrapper
        {
            get
            {
                if (null == _pointOfInterestRepositoryWrapper)
                {
                    _pointOfInterestRepositoryWrapper = new PointOfInterestRepository(_repoContext);
                }

                return (_pointOfInterestRepositoryWrapper);
            }
        }

        public async Task<int> Save()
        {
          int NumberOfObjectsChanged = 0;
          NumberOfObjectsChanged = await _repoContext.SaveChangesAsync();

          return (NumberOfObjectsChanged);
        }
    }
}
