using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesContracts
{
  public interface ICityLanguage
  {
    Task<ICommunicationResults> UpdateCityLanguagesList(List<CityLanguageForSaveAndUpdateDto> CityLanguageDto_Object_List,
                                                        bool DeleteOldElementsInListNotSpecifiedInCurrentList,
                                                        string UserName = "No Name");
  }
}
