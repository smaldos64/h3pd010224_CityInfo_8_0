using CityInf0_8_0_Server;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
    public class CityRepositoryTest1
    {
        protected DbContextOptions<DatabaseContext> _contextOptions { get; }

        private DatabaseContext _databaseContext;
        private DbContextOptions<DatabaseContext> _dbContextoptions;

        protected TestServer _testServer;

        private IRepositoryWrapper _repositoryWrapper { get; }

        //private void SeedData(DatabaseContext context)
        private void SeedData()
        {
            using (var context = new DatabaseContext(this._dbContextoptions))
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

        //public CityRepositoryTest1(DbContextOptions<DatabaseContext> contextOptions,
        //                           IRepositoryWrapper repositoryWrapper)
        //{
        //    this._contextOptions = contextOptions;
        //    this._dbContextoptions = contextOptions;
        //    this._databaseContext = new DatabaseContext(this._contextOptions);
        //    this._repositoryWrapper = repositoryWrapper;
        //    SeedData();
        //}

        public CityRepositoryTest1(TestServer testServer)
        {
            //this._contextOptions = contextOptions;
            //this._dbContextoptions = contextOptions;
            //this._databaseContext = new DatabaseContext(this._contextOptions);
            this._testServer = testServer;
            this._repositoryWrapper = testServer.Services.GetService<IRepositoryWrapper>();
            //SeedData();
        }

        //public CityRepositoryTest1(DbContextOptions<DatabaseContext> contextOptions,
        //                           TestServer testServer)
        //{
        //    this._contextOptions = contextOptions;
        //    this._dbContextoptions = contextOptions;
        //    this._databaseContext = new DatabaseContext(this._contextOptions);
        //    this._testServer = testServer;
        //    this._repositoryWrapper = testServer.Services.GetService<IRepositoryWrapper>();
        //    //SeedData();
        //}

        //public CityRepositoryTest1(DbContextOptions<DatabaseContext> contextOptions,
        //                           TestServer testServer)
        //{
        //    this._contextOptions = contextOptions;
        //    this._dbContextoptions = contextOptions;
        //    this._databaseContext = new DatabaseContext(this._contextOptions);
        //    this._testServer = testServer;
        //    this._repositoryWrapper = testServer.Services.GetService<IRepositoryWrapper>();
        //    //SeedData();
        //}

        //protected CityRepositoryTest1(DbContextOptions<DatabaseContext> options) 
        //{
        //    this._contextOptions = options;
        //    this._databaseContext = new DatabaseContext(this._contextOptions);
        //    this._repositoryWrapper = new RepositoryWrapper();
        //    SeedData();
        //}

        [Fact]
        public async void Test_CityRepository_GetAllCities1()
        {
            // Arrange
            //var controller = new RegistrationController(context);

            // Act
            IEnumerable<City> CityList = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(false);
            List<City> cities = CityList.ToList();

            // Assert
            Assert.Equal(3, cities.Count);
        }
    }
}
