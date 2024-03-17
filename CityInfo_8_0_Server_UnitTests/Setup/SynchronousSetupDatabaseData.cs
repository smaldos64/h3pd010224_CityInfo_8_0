using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_UnitTests.Setup
{
    public static class SynchronousSetupDatabaseData
    {
        public static List<Language> LanguageObjectList = new List<Language>();
        public static List<Country> CountryObjectList = new List<Country>();
        public static List<City> CityObjectList = new List<City>();
        public static List<PointOfInterest> PointOfInterestObjectList = new List<PointOfInterest>();
        public static List<CityLanguage> CityLanguageObjectList = new List<CityLanguage>();

        public static void SeedDatabaseData(DatabaseContext context)
        {
            int NumberOfDatabaseObjectsChanged = 0;

            LanguageObjectList = new List<Language>()
                {
                    new Language
                    {
                        LanguageName = "Dansk"
                    },
                    new Language
                    {
                        LanguageName = "Engelsk"
                    },
                    new Language
                    {
                        LanguageName = "Tysk"
                    }
                };
            context.AddRange(LanguageObjectList);
            NumberOfDatabaseObjectsChanged = context.SaveChanges();

            CountryObjectList = new List<Country>()
                {
                    new Country
                    {
                        CountryName = "Danmark"
                    },
                    new Country
                    {
                        CountryName = "England"
                    },
                    new Country
                    {
                        CountryName = "Tyskland"
                    },
                };
            context.AddRange(CountryObjectList);
            NumberOfDatabaseObjectsChanged = context.SaveChanges();

            CityObjectList = new List<City>()
                {
                    new City
                    {
                        CityName = "Gudumholm",
                        CityDescription = "Østhimmerlands perle !!!",
                        CountryID = CountryObjectList[0].CountryID
                    },
                    new City
                    {
                        CityName = "London",
                        CityDescription = "Englands hovedstad",
                        CountryID = CountryObjectList[1].CountryID
                    },
                    new City
                    {
                        CityName = "Hamburg",
                        CityDescription = "Byen ved Elben",
                        CountryID = CountryObjectList[2].CountryID
                    }
                };
            context.AddRange(CityObjectList);
            NumberOfDatabaseObjectsChanged = context.SaveChanges();

            PointOfInterestObjectList = new List<PointOfInterest>()
                {
                    new PointOfInterest
                    {
                        PointOfInterestName = "Gudumholm Stadion",
                        PointOfInterestDescription = "Her har Lars P spillet mange kampe",
                        CityId = CityObjectList[0].CityId
                    },
                    new PointOfInterest
                    {
                        PointOfInterestName = "Gudumholm Brugs",
                        PointOfInterestDescription = "Her regerer Jesper Baron Berthelsen",
                        CityId = CityObjectList[0].CityId
                    },
                    new PointOfInterest
                    {
                        PointOfInterestName = "Wembley",
                        PointOfInterestDescription = "Berømt fodboldstadion",
                        CityId = CityObjectList[1].CityId
                    },
                    new PointOfInterest
                    {
                        PointOfInterestName = "Elben tunnellen",
                        PointOfInterestDescription = "Letter trafikken gennem Hamburg",
                        CityId = CityObjectList[2].CityId
                    }
                };
            context.AddRange(PointOfInterestObjectList);
            NumberOfDatabaseObjectsChanged = context.SaveChanges();

            CityLanguageObjectList = new List<CityLanguage>()
                {
                    new CityLanguage
                    {
                        CityId = CityObjectList[0].CityId,
                        LanguageId = LanguageObjectList[0].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = CityObjectList[0].CityId,
                        LanguageId = LanguageObjectList[1].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = CityObjectList[0].CityId,
                        LanguageId = LanguageObjectList[2].LanguageId
                    },

                    new CityLanguage
                    {
                        CityId = CityObjectList[1].CityId,
                        LanguageId = LanguageObjectList[1].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = CityObjectList[1].CityId,
                        LanguageId = LanguageObjectList[2].LanguageId
                    },

                    new CityLanguage
                    {
                        CityId = CityObjectList[2].CityId,
                        LanguageId = LanguageObjectList[1].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = CityObjectList[2].CityId,
                        LanguageId = LanguageObjectList[2].LanguageId
                    },
                };
            context.AddRange(CityLanguageObjectList);
            NumberOfDatabaseObjectsChanged = context.SaveChanges();

            //var Cities = context.Core_8_0_Cities.ToList();
            CityObjectList = context.Core_8_0_Cities.ToList();
        }
    }
}
