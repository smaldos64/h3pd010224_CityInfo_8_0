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
        [Required(ErrorMessage = "You should provide a name value.")]
        [MaxLength(50)]
        public string CityName { get; set; }

        [MaxLength(200)]
        public string CityDescription { get; set; }
    }

    public class CityForSaveWithCountryDto : CityForSaveDto
    {
        public virtual int CountryID { get; set; }
    }

    public class CityForUpdateDto : CityForSaveWithCountryDto
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

        public CountryDto Country { get; set; }
    }
        
    public class CityDto : CityDtoMinusRelations
    {
        public ICollection<LanguageDtoMinusRelations> CityLanguages { get; set; }
               = new List<LanguageDtoMinusRelations>();
    }
}
