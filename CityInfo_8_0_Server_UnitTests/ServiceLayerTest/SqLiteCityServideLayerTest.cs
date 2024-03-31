using CityInfo_8_0_TestSetup.Assertions;
using CityInfo_8_0_TestSetup.Database;
using Contracts;
using Entities.Models;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using CityInfo_8_0_TestSetup.Setup;
using Services;
using ServicesContracts;
using CityInfo_8_0_TestSetup.ViewModels;

namespace CityInfo_8_0_Server_UnitTests.ServiceLayerTest
{
    public class SqLiteCityServideLayerTest : IDisposable
    {
        private DbConnection _connection;
        private DbContextOptions<DatabaseContext> _contextOptions;
        private IRepositoryWrapper _repositoryWrapper;
        private ICityService _cityService;
        private ICityLanguageService _cityLanguage;
        private IPointOfInterestService _pointOfInterestService;
        private DatabaseViewModel _databaseViewModel;

        public SqLiteCityServideLayerTest()
        {
            Task.Run(async () =>
            {
                // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
                // at the end of the test (see Dispose below).
                _connection = new SqliteConnection("Filename=:memory:");
                //_connection = new SqliteConnection("Filename=:" + Guid.NewGuid().ToString() + ":");
                //_connection = new SqliteConnection("Data Source = " + Guid.NewGuid().ToString());
                _connection.Open();

                _contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite(_connection)
                .Options;

                var context = new UnitTestDatabaseContext(_contextOptions, null);

                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                //_connection.Open();
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

        public void Dispose()
        {
            _connection.DisposeAsync();
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task SqLite_Test_CityService_GetAllCities_Using_CityService(bool includeRelations)
        {
            // Arrange
            _repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();

            // Act
            IEnumerable<City> CityIEnumerable = await _cityService.GetAllCities(includeRelations);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            bool CompareResult = CustomAssert.AreListOfObjectsEqualByFields<City>(CityList,
                                                                                  _databaseViewModel.CityList,
                                                                                  false);
            Assert.True(CompareResult);
        }
    }
}
