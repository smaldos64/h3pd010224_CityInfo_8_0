using Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CityInfo_8_0_Server_UnitTests.Setup;
using Entities.Models;
using Repository;
using CityInfo_8_0_Server_UnitTests.Database;
using Contracts;
using CityInfo_8_0_Server_UnitTests.Assertions;
using NLog.Common;
using CityInfo_8_0_Server_UnitTests.ViewModels;

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
    public class InMemoryCityRepositoryLayerTest
    {
        private DbContextOptions<DatabaseContext> _contextOptions;
        private ICityRepository _cityRepository;   
        private IRepositoryWrapper _repositoryWrapper;
        private DatabaseViewModel _databaseViewModel;

        public InMemoryCityRepositoryLayerTest()
        {
            Task.Run(async () =>
            {
                _contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                //.UseInMemoryDatabase("InMemoryRepositoryTest")
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging(true)
                .EnableDetailedErrors(true)
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            
                var context = new UnitTestDatabaseContext(_contextOptions, null);
                            
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                _databaseViewModel = new DatabaseViewModel();
                await SetupDatabaseData.SeedDatabaseDataWithObject(context, _databaseViewModel);
                //await SetupDatabaseData.SeedDatabaseData(context);

                _cityRepository = new CityRepository(context);
                _repositoryWrapper = new RepositoryWrapper(context);
            }).GetAwaiter().GetResult();
            
            //await SetupDatabaseData.SeedDatabaseData(context);

            //_cityRepository = new CityRepository(context);
            //_repositoryWrapper = new RepositoryWrapper(context);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task InMemory_Test_CityRepository_GetAllCities_Using_CityRepository(bool includeRelations)
        {
            // Arrange
                        
            // Act
            IEnumerable<City> CityIEnumerable = await _cityRepository.GetAllCities(includeRelations);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            //await CustomAssert.InMemoryModeCheckCitiesRead(CityList, includeRelations);
            await CustomAssert.InMemoryModeCheckCitiesReadWithObject(CityList, _databaseViewModel, includeRelations);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task InMemory_Test_CityRepository_GetAllCities_Using_RepositoryWrapper(bool includeRelations)
        {
            // Arrange

            // Act
            IEnumerable<City> CityIEnumerable = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(false);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            //await CustomAssert.InMemoryModeCheckCitiesRead(CityList, includeRelations);
            await CustomAssert.InMemoryModeCheckCitiesReadWithObject(CityList, _databaseViewModel, includeRelations);
        }
    }
}
