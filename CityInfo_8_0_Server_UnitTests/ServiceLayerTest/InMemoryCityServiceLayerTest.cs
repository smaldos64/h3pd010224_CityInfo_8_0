using CityInfo_8_0_TestSetup.Assertions ;
using CityInfo_8_0_TestSetup.Database;
using CityInfo_8_0_TestSetup.Setup;
using CityInfo_8_0_TestSetup.ViewModels;
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
        private DbContextOptions<DatabaseContext> _contextOptions;
        private IRepositoryWrapper _repositoryWrapper;
        private ICityService _cityService;
        private ICityLanguageService _cityLanguage;
        private IPointOfInterestService _pointOfInterestService;
        private DatabaseViewModel _databaseViewModel;

        public InMemoryCityServiceLayerTest()
        {
            Task.Run(async () =>
            {
                _contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

                var context = new UnitTestDatabaseContext(_contextOptions, null);

                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                _databaseViewModel = new DatabaseViewModel();
                await SetupDatabaseData.SeedDatabaseDataWithObject(context, _databaseViewModel);

                _repositoryWrapper = new RepositoryWrapper(context);
                _cityLanguage = new CityLanguageService(_repositoryWrapper);
                _pointOfInterestService = new PointOfInterestService(_repositoryWrapper);
                _cityService = new CityService(_repositoryWrapper,
                                               _cityLanguage,
                                               _pointOfInterestService);
            }).GetAwaiter().GetResult();
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task InMemory_Test_CityService_GetAllCities_Using_CityService(bool includeRelations)
        {
            // Arrange
            _repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();

            // Act
            IEnumerable<City> CityIEnumerable = await _cityService.GetAllCities(includeRelations);
            List<City> CityList = CityIEnumerable.ToList();

            List<City> CityListSorted = new List<City>();
            CityListSorted = CityList.OrderBy(c => c.CityId).ToList();

            // Assert
            bool CompareResult = CustomAssert.AreListOfObjectsEqualByFields<City>(CityListSorted,
                                                                                  _databaseViewModel.CityList,
                                                                                  false);
            Assert.True(CompareResult);
        }
    }
}
