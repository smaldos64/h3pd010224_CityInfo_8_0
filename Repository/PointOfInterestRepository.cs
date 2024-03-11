using Entities;
using Entities.Models;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class PointOfInterestRepository : RepositoryBase<PointOfInterest>, IPointOfInterestRepository
    {
        public PointOfInterestRepository(DatabaseContext context) : base(context)
        {
            if (null == context)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
        public async Task<IEnumerable<PointOfInterest>> GetAllPointOfInterestWithLanguageID(int LanguageID, bool UseLazyLoading)
        {
            if (true == UseLazyLoading)
            {
                this.databaseContext.ChangeTracker.LazyLoadingEnabled = true;
                var collection = await (base.FindByCondition(x => x.City.CityLanguages.Any(cl => cl.LanguageId == LanguageID))); // LTPE => distinct ???

                return (collection.ToList());
            }
            else
            {
                this.databaseContext.ChangeTracker.LazyLoadingEnabled = false;
                
                var collection = await base.databaseContext.Core_8_0_PointsOfInterest.
                   Include(c => c.City).
                   ThenInclude(co => co.Country).
                   Include(c => c.City).
                   ThenInclude(c => c.CityLanguages).
                   ThenInclude(l => l.Language).
                   Where(c => c.City.CityLanguages.Any(cl => cl.LanguageId == LanguageID)).Distinct().ToListAsync();

                return (collection);
            }
        }

        public async Task<IEnumerable<PointOfInterest>> GetAllPointOfInterestWithCityID(int CityID, bool UseLazyLoading)
        {
            var collection = await base.FindByCondition(c => c.CityId == CityID);

            return (collection.ToList());
        }
    }
}
