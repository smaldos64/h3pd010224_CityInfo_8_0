using CityInfo_8_0_Server_UnitTests.Assertions;
using CityInfo_8_0_Server_UnitTests.Database;
using CityInfo_8_0_Server_UnitTests.Setup;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Repository;
using Services;
using ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CityInfo_8_0_Server_UnitTests.ServiceLayerTest
{
    public class InMemoryCityServiceLayerTest
    {
        private readonly DbContextOptions<DatabaseContext> _contextOptions;
        //private readonly CityRepository _cityRepository;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ICityService _cityService;
        private readonly ICityLanguageService _cityLanguage;
        private readonly IPointOfInterestService _pointOfInterestService;

        public InMemoryCityServiceLayerTest()
        {
            _contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("BloggingControllerTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

            var context = new UnitTestDatabaseContext(_contextOptions, null);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            SetupDatabaseData.SeedDatabaseData(context);

            _repositoryWrapper = new RepositoryWrapper(context);
            _cityLanguage = new CityLanguageService(_repositoryWrapper);
            _pointOfInterestService = new PointOfInterestService(_repositoryWrapper);
            _cityService = new CityService(_repositoryWrapper,
                                           _cityLanguage,
                                           _pointOfInterestService);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async void Test_CityService_GetAllCities_Using_CityService(bool includeRelations)
        {
            // Arrange
            _repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();

            // Act
            IEnumerable<City> CityIEnumerable = await _cityService.GetAllCities(includeRelations);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            CustomAssert.InMemoryModeCheckCitiesRead(CityList, includeRelations);
            
            //Assert.Equal(3, CityList.Count);

            //if (true == includeRelations) 
            //{
            //    for (int Counter = 0; Counter < SetupDatabaseData.CityObjectList.Count; Counter++)
            //    {
            //        Assert.Equal(SetupDatabaseData.CityObjectList[Counter].CityLanguages.Count,
            //        CityList[Counter].CityLanguages.Count);
            //    }
            //}
            //else
            //{
            //    // Det kan åbenbart ikke rigtig lade sig gøre at få
            //    // InMemory databasen til at holde op med at bruge
            //    // LazyLoading, selvom vi længere oppe i vores testCase 
            //    // har specificeret, at LazyLoading skal disables.

            //    //for (int Counter = 0; Counter < SetupDatabaseData.CityObjectList.Count; Counter++)
            //    //{
            //    //    Assert.Equal(0, CityList[Counter].CityLanguages.Count);
            //    //}
            //    for (int Counter = 0; Counter < SetupDatabaseData.CityObjectList.Count; Counter++)
            //    {
            //        Assert.Equal(SetupDatabaseData.CityObjectList[Counter].CityLanguages.Count,
            //        CityList[Counter].CityLanguages.Count);
            //    }
            //}
        }
    }
}
