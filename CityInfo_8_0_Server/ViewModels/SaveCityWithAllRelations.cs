using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server.ViewModels
{
    public class SaveCityWithAllRelations : CityWithAllRelations
    {
        public CityForSaveWithCountryDto CityDto_Object { get; set; }

        public List<PointOfInterestForSaveWithCityDto> PointOfInterests { get; set; }
            = new List<PointOfInterestForSaveWithCityDto>();
    }
}
