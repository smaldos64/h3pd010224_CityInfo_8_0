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
        public string PointOfInterestName { get; set; }

        public string PointOfInterestDescription { get; set; }

        public int CityId { get; set; }
    }

    public class PointOfInterestForUpdateDto : PointOfInterestForSaveDto
    {
        public int PointOfInterestId { get; set; }
    }

    public class PointOfInterestDto : PointOfInterestForUpdateDto
    {
        public CityDto City { get; set; }
    }
}
