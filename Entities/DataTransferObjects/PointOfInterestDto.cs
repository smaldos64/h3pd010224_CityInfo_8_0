using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Entities.Models;

namespace Entities.DataTransferObjects
{
    public class PointOfInterestForSaveDto
    {
        [Required(ErrorMessage = "You should provide a name value.")]
        [MaxLength(50)]
        public string PointOfInterestName { get; set; }

        [MaxLength(200)]
        public string PointOfInterestDescription { get; set; }
    }

    public class PointOfInterestForSaveWithCityDto : PointOfInterestForSaveDto
    {
        public int CityId { get; set; }
    }

    public class PointOfInterestForUpdateDto : PointOfInterestForSaveWithCityDto
    {
        public int PointOfInterestId { get; set; }

        //public int CityID { get; set; }
        // Navigation Property => ingen grund til at have dette felt med i vores
        // DTO model !!!
    }

    public class PointOfInterestDto : PointOfInterestForUpdateDto
    {
        public CityDto City { get; set; }
    }
}
