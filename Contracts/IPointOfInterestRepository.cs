using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPointOfInterestRepository : IRepositoryBase<PointOfInterest>
    {
        Task<IEnumerable<PointOfInterest>> GetAllPointOfInterestWithLanguageID(int LanguageID, bool UseLazyLoading);

        Task<IEnumerable<PointOfInterest>> GetAllPointOfInterestWithCityID(int LanguageID, bool UseLazyLoading);
    }
}
