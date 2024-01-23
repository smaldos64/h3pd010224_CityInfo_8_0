using Entities.DataTransferObjects;
using System.Collections.Generic;

namespace CityInfo_8_0_Server.ViewModels
{
    public class CityWithAllRelations
    {
        public List<CityLanguageForSaveAndUpdateDto> CityLanguages { get; set; }
              = new List<CityLanguageForSaveAndUpdateDto>();
    }
}
