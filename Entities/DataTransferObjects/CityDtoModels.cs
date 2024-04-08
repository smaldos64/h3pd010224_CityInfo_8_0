using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Entities.Models;

namespace Entities.DataTransferObjects
{
    public class CityForSaveDto
    {
        public string CityName { get; set; }

        public string CityDescription { get; set; }

        public virtual int CountryID { get; set; }
    }

    public class CityForUpdateDto : CityForSaveDto
    {
        public int CityId { get; set; }
    }

    public class CityDtoPointsOfInterests : CityForUpdateDto
    {
        public int NumberOfPointsOfInterest
        {
            get
            {
                return PointsOfInterest.Count;
            }
        }

        public ICollection<PointOfInterestForUpdateDto> PointsOfInterest { get; set; }
          = new List<PointOfInterestForUpdateDto>();
    }

    public class CityDtoMinusRelations : CityDtoPointsOfInterests
    {
        public override int CountryID { get; set; }

        public CountryDtoMinusRelations Country { get; set; }
    }

    public class CityDto : CityDtoMinusRelations
    {
        public ICollection<CityLanguageDtoMinusCityRelations> CityLanguages { get; set; }
                  = new List<CityLanguageDtoMinusCityRelations>();
    }

    public class CityDtoMinusCountryRelations : CityDtoPointsOfInterests
    {
        public ICollection<CityLanguageDtoMinusCityRelations> CityLanguages { get; set; }
                  = new List<CityLanguageDtoMinusCityRelations>();
    }
}
