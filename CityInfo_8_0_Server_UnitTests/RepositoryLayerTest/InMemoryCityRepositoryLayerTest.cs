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

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
    public class InMemoryCityRepositoryLayerTest
    {
        private readonly DbContextOptions<DatabaseContext> _contextOptions;
        private readonly ICityRepository _cityRepository;   
        private readonly IRepositoryWrapper _repositoryWrapper;

        public InMemoryCityRepositoryLayerTest()
        {
            _contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("BloggingControllerTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
            
            var context = new UnitTestDatabaseContext(_contextOptions, null);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            SetupDatabaseData.SeedDatabaseData(context);

            _cityRepository = new CityRepository(context);
            _repositoryWrapper = new RepositoryWrapper(context);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async void InMemory_Test_CityRepository_GetAllCities_Using_CityRepository(bool includeRelations)
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
        public async void InMemory_Test_CityRepository_GetAllCities_Using_RepositoryWrapper(bool includeRelations)
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
