using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesContracts
{
  public interface IPointOfInterestService
  {
    Task<ICommunicationResults> UpdatePointOfInterestListForCity(int CityId,
                                                                 List<PointOfInterestForUpdateDto> PointOfInterestForUpdateDto_List,
                                                                 bool DeleteOldElementsInListNotSpecifiedInCurrentList = true,
                                                                 string UserName = "No Name",
                                                                 bool UseExtendedDatabaseDebugging = false);
  }
}
