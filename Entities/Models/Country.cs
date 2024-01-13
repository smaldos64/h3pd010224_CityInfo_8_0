using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountryID { get; set; }

        [Required]
        [MaxLength(50)]
        public string CountryName { get; set; }

        public virtual ICollection<City> Cities { get; set; }
               = new List<City>();
    }
}
