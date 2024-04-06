using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class CountryForSaveDto
    {
        [Required]
        [MaxLength(50)]
        public string CountryName { get; set; }
    }

    public class CountryForUpdateDto : CountryForSaveDto
    {
        public int CountryID { get; set; }

        //public virtual ICollection<CityDto> Cities { get; set; }
        //       = new List<CityDto>();
    }

    public class CountryDtoMinusRelations : CountryForUpdateDto
    {

    }

    public class CountryDto : CountryForUpdateDto
    {
        public ICollection<CityDtoMinusCountryRelations> Cities { get; set; }
               = new List<CityDtoMinusCountryRelations>();
    }
}
