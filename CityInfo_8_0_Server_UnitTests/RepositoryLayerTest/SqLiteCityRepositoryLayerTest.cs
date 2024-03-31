using CityInfo_8_0_TestSetup.Assertions;
using CityInfo_8_0_TestSetup.Database;
using Contracts;
using Entities.Models;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Data.Sqlite;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityInfo_8_0_TestSetup.Setup;
using System.Data.Common;
using CityInfo_8_0_TestSetup.ViewModels;

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
    public class SqLiteCityRepositoryLayerTest : IDisposable
    {
        private DbConnection _connection;
        private DbContextOptions<DatabaseContext> _contextOptions;
        private ICityRepository _cityRepository;
        private IRepositoryWrapper _repositoryWrapper;
        private DatabaseViewModel _databaseViewModel;

        public SqLiteCityRepositoryLayerTest()
        {
            Task.Run(async () =>
            {
                // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
                // at the end of the test (see Dispose below).
                _connection = new SqliteConnection("Filename=:memory:");
                _connection.Open();

                _contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite(_connection)
                .Options;

                var context = new UnitTestDatabaseContext(_contextOptions, null);

                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                _databaseViewModel = new DatabaseViewModel();
                await SetupDatabaseData.SeedDatabaseDataWithObject(context, _databaseViewModel);

                _cityRepository = new CityRepository(context);
                _repositoryWrapper = new RepositoryWrapper(context);
            }).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task SqLite_Test_CityRepository_GetAllCities_Using_CityRepository(bool includeRelations)
        {
            // Arrange

            // Act
            IEnumerable<City> CityIEnumerable = await _cityRepository.GetAllCities(includeRelations);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            bool CompareResult = CustomAssert.AreListOfObjectsEqualByFields<City>(CityList,
                                                                                  _databaseViewModel.CityList,
                                                                                  false);
            Assert.True(CompareResult);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task SqLite_Test_CityRepository_GetAllCities_Using_RepositoryWrapper(bool includeRelations)
        {
            // Arrange

            // Act
            IEnumerable<City> CityIEnumerable = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(false);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            //await CustomAssert.InMemoryModeCheckCitiesReadWithObject(CityList, _databaseViewModel, includeRelations);
            bool CompareResult = CustomAssert.AreListOfObjectsEqualByFields<City>(CityList,
                                                                                  _databaseViewModel.CityList,
                                                                                  false);
            Assert.True(CompareResult);
        }
    }
}
