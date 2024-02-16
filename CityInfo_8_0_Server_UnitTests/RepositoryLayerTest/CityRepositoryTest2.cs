using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
    public class CityRepositoryTest2
    {
        private IRepositoryWrapper _repositoryWrapper;
        private DatabaseContext _databaseContext;
        private DbContextOptions<DatabaseContext> _dbContextoptions;

        private void SeedData()
        {
            //using (var context = new DatabaseContext(this._dbContextoptions))
            using (var context = this._databaseContext)
            {
                List<Language> LanguageObjectList = new List<Language>()
                {
                    new Language
                    {
                        //LanguageId = 1,
                        LanguageName = "dansk"
                    },
                    new Language
                    {
                        //LanguageId = 2,
                        LanguageName = "engelsk"
                    },
                    new Language
                    {
                        //LanguageId = 3,
                        LanguageName = "tysk"
                    }
                };
                context.AddRangeAsync(LanguageObjectList);
                context.SaveChanges();

                List<Country> CountryObjectList = new List<Country>()
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
                context.AddRangeAsync(CountryObjectList);
                context.SaveChanges();

                List<City> CityObjectList = new List<City>()
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
                context.AddRangeAsync(CityObjectList);
                context.SaveChanges();

                List<PointOfInterest> PointOfInterestObjectList = new List<PointOfInterest>()
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
                context.AddRangeAsync(PointOfInterestObjectList);
                context.SaveChanges();

                List<CityLanguage> CityLanguageObjectList = new List<CityLanguage>()
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
                context.AddRangeAsync(CityLanguageObjectList);
                context.SaveChanges();

                var Cities = context.Core_8_0_Cities.ToList();
            }
        }

        //public CityRepositoryTest2(IRepositoryWrapper repositoryWrapper)
        public CityRepositoryTest2()
        {
            this._dbContextoptions = (new DbContextOptionsBuilder<DatabaseContext>()
                                    .UseInMemoryDatabase("UnitTestDatabase")
                                    .Options);

            this._databaseContext = new DatabaseContext(this._dbContextoptions);
            //this._repositoryWrapper = repositoryWrapper;
            
            SeedData();
        }

        [Fact]
        public async void Test_CityRepository_GetAllCities_2()
        {
            // Arrange
            //var controller = new RegistrationController(context);
            CityRepository CityRepositoryObject = new CityRepository(this._databaseContext);

            // Act
            //IEnumerable<City> CityList = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(false);
            IEnumerable<City> CityList = await CityRepositoryObject.GetAllCities(false);
            List<City> cities = CityList.ToList();

            // Assert
            Assert.Equal(3, cities.Count);
        }


    }
}
