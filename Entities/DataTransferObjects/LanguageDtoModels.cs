using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{

    public class LanguageForSaveDto
    {
        public string LanguageName { get; set; }
    }

    public class LanguageForUpdateDto : LanguageForSaveDto
    {
        public int LanguageId { get; set; }
    }

    public class LanguageDtoMinusRelations : LanguageForUpdateDto
    {
    }

    public class LanguageDto : LanguageDtoMinusRelations
    {
        public ICollection<CityLanguageDtoMinusLanguageRelations> CityLanguages { get; set; }
              = new List<CityLanguageDtoMinusLanguageRelations>();
    }

}
