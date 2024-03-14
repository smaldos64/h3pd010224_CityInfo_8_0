using CityInfo_8_0_Server_UnitTests.Assertions;
using CityInfo_8_0_Server_UnitTests.Database;
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
using CityInfo_8_0_Server_UnitTests.Setup;
using System.Data.Common;

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
    public class SqLiteCityRepositoryLayerTest : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<DatabaseContext> _contextOptions;
        private readonly ICityRepository _cityRepository;
        private readonly IRepositoryWrapper _repositoryWrapper;

        public SqLiteCityRepositoryLayerTest()
        {
            // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
            // at the end of the test (see Dispose below).
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            //.UseInMemoryDatabase("BloggingControllerTest")
            //.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .UseSqlite(_connection)
            .Options;

            var context = new UnitTestDatabaseContext(_contextOptions, null);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            SetupDatabaseData.SeedDatabaseData(context);

            _cityRepository = new CityRepository(context);
            _repositoryWrapper = new RepositoryWrapper(context);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async void Test_CityRepository_GetAllCities_Using_CityRepository(bool includeRelations)
        {
            // Arrange

            // Act
            IEnumerable<City> CityIEnumerable = await _cityRepository.GetAllCities(includeRelations);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            CustomAssert.InMemoryModeCheckCitiesRead(CityList, includeRelations);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async void Test_CityRepository_GetAllCities_Using_RepositoryWrapper(bool includeRelations)
        {
            // Arrange

            // Act
            IEnumerable<City> CityIEnumerable = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(false);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            CustomAssert.InMemoryModeCheckCitiesRead(CityList, includeRelations);
        }
    }
}
