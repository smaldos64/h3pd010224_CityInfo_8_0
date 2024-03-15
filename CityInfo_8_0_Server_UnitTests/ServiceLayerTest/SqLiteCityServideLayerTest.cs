﻿using CityInfo_8_0_Server_UnitTests.Assertions;
using CityInfo_8_0_Server_UnitTests.Database;
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
using CityInfo_8_0_Server_UnitTests.Setup;
using Services;
using ServicesContracts;

namespace CityInfo_8_0_Server_UnitTests.ServiceLayerTest 
{
    public class SqLiteCityServideLayerTest : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<DatabaseContext> _contextOptions;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ICityService _cityService;
        private readonly ICityLanguageService _cityLanguage;
        private readonly IPointOfInterestService _pointOfInterestService;

        public SqLiteCityServideLayerTest()
        {
            // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
            // at the end of the test (see Dispose below).
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(_connection)
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

        public void Dispose()
        {
            _connection.Dispose();
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async void SqLite_Test_CityService_GetAllCities_Using_CityService(bool includeRelations)
        {
            // Arrange
            _repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();

            // Act
            IEnumerable<City> CityIEnumerable = await _cityService.GetAllCities(includeRelations);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            CustomAssert.InMemoryModeCheckCitiesRead(CityList, includeRelations);
        }
    }
}
