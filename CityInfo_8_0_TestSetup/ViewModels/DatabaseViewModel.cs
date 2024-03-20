using Entities.DataTransferObjects;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_TestSetup.ViewModels
{
    public class DatabaseViewModel
    {
        public List<City> CityList { get; set; } = new List<City>();

        public List<Language> LanguageList { get; set; } = new List<Language>();
        public List<Country> CountryList { get; set; }  = new List<Country>();
        public List<PointOfInterest> PointOfInterestList { get; set;} = new List<PointOfInterest>();
        public List<CityLanguage> CityLanguageList { get; set; } = new List<CityLanguage>();
    }
}
