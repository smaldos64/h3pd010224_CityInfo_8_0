using Entities.DataTransferObjects;
using System.Collections.Generic;

namespace CityInfo_8_0_Server.ViewModels
{
    public class UpdateCityWithAllRelations : CityWithAllRelations
    {
        public CityForUpdateDto CityDto_Object { get; set; }

        public List<PointOfInterestForUpdateDto> PointOfInterests { get; set; }
            = new List<PointOfInterestForUpdateDto>();
    }
}
