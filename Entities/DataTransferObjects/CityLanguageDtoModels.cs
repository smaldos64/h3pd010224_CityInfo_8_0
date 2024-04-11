using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Entities.Models;

namespace Entities.DataTransferObjects
{

    public class CityLanguageForSaveAndUpdateDto
    {
        public int CityId { get; set; }

        public int LanguageId { get; set; }
    }
    
    public class CityLanguageDto : CityLanguageForSaveAndUpdateDto
    {
        public CityDtoMinusRelations City { get; set; }

        public LanguageDtoMinusRelations Language { get; set; }
    }

    public class CityLanguageDtoMinusCityRelations
    {
        public int LanguageId { get; set; }
        public LanguageDtoMinusRelations Language { get; set; }
    }

    public class CityLanguageDtoMinusLanguageRelations
    {
        public int CityId { get; set; }
        public CityDtoMinusRelations City { get; set; }
    }
}
