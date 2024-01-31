using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter.Xml;
using Entities.DataTransferObjects;
using Entities.Models;

namespace ServicesContracts
{
  public interface ICityService
  {
    Task<IEnumerable<City>> GetCities(bool IncludeRelations = false);
    Task<int> SaveCity(City City_Object);

    Task<ICommunicationResults> UpdateCityWithAllRelations(CityForUpdateDto CityForUpdateDto_Object,
                                                           List<PointOfInterestForUpdateDto> PointOfInterestForUpdateDto_List,
                                                           List<CityLanguageForSaveAndUpdateDto> CityLanguageForSaveAndUpdateDto_list,
                                                           string UserName = "No Name",
                                                           bool DeleteOldElementsInListsNotSpecifiedInCurrentLists = true,
                                                           bool UseExtendedDatabaseDebugging = false);
  }
}
