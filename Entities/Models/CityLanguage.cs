using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class CityLanguage
    {
        [Required]
        [Key]
        public int CityId { get; set; }
     
        [Required]
        [Key]
        public int LanguageId { get; set; }

        public virtual City City { get; set; }

        public virtual Language Language { get; set; }
    }
}
