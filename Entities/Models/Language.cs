using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    // Database model
    public class Language
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LanguageId { get; set; }
        [Required]
        [MaxLength(50)]
        public string LanguageName { get; set; }

        public virtual ICollection<CityLanguage> CityLanguages { get; set; }
               = new List<CityLanguage>();
    }

    // DTO modeller
    //public class LanguageForSaveDto
    //{
    //    [Required]
    //    [MaxLength(100)]
    //    //public virtual string LanguageName { get; set; }
    //    public string LanguageName { get; set; }
    //}

    //public class LanguageForUpdateDto : LanguageForSaveDto
    //{
    //    public int LanguageId { get; set; }
    //}
    
    //public class LanguageDtoMinusRelations : LanguageForUpdateDto
    //{
    //}

    //public class LanguageDto : LanguageDtoMinusRelations
    //{
    //    public ICollection<CityDto> CityLanguages { get; set; }
    //          = new List<CityDto>();
    //}
}
