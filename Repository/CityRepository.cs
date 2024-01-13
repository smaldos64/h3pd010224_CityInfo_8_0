using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Entities.Models;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CityRepository : RepositoryBase<City>, ICityRepository
    {
        public CityRepository(DatabaseContext context) : base(context)
        {
            if (null == context)
            {
                throw new ArgumentNullException(nameof(context));
            }
            this.RepositoryContext.ChangeTracker.LazyLoadingEnabled = false;
        }

        public async Task<IEnumerable<City>> GetAllCities(bool IncludeRelations = false)
        {
            if (false == IncludeRelations)
            {
                var collection = await (base.FindAll());
                collection = collection.OrderByDescending(c => c.CityLanguages.Count).ThenBy(c => c.CityName);
                return (collection);
            }
            else
            {
                var collection = await base.RepositoryContext.Core_8_0_Cities.
                Include(c => c.PointsOfInterest).
                Include(co => co.Country).
                Include(c => c.CityLanguages).
                ThenInclude(l => l.Language).ToListAsync();

                var collection1 = collection.OrderByDescending(c => c.CityLanguages.Count).ThenBy(c => c.CityName);
                return (collection1);
            }
        }

        public async Task<City> GetCity(int CityId, bool IncludeRelations = false)
        {
            if (false == IncludeRelations)
            {
                var City_Object = base.FindOne(CityId);
                return await (City_Object);
            }
            else
            {
                //var City_Object = base.FindAll().Where(c => c.Id == CityId).
                var City_Object = await base.RepositoryContext.Core_8_0_Cities.Include(c => c.PointsOfInterest).
                Include(c => c.PointsOfInterest).
                Include(c => c.CityLanguages).
                ThenInclude(l => l.Language).
                FirstOrDefaultAsync();

                return (City_Object);
            }
        }

#if OLD_IMPLEMENTATION
        public Async Task <IEnumerable<City>> GetCitiesFromLanguages(int languageID)
        {
            return RepositoryContext.Cities.Include(x => x.CityLanguages).ThenInclude(x => x.Language).Include(x => x.PointsOfInterest).Where(x => x.CityLanguages.Any(cl => cl.LanguageId == languageID));
        }
#else
        public async Task<IEnumerable<City>> GetCitiesFromLanguageID(int languageID)
        {
            var collection = await base.FindByCondition(x => x.CityLanguages.Any(cl => cl.LanguageId == languageID));

            collection = collection.OrderByDescending(c => c.CityLanguages.Count).ThenBy(c => c.CityName);
            
            return (collection.ToList());
        }
#endif
    }
}
