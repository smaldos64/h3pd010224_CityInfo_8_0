using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PointOfInterestId { get; set; }
        [Required]
        [MaxLength(50)]
        public string PointOfInterestName { get; set; }
        [MaxLength(200)]
        public string PointOfInterestDescription { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        public int CityId { get; set; }

    }
}
