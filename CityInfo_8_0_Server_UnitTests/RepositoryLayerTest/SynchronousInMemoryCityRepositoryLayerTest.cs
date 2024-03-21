using CityInfo_8_0_TestSetup.Assertions;
using CityInfo_8_0_TestSetup.Database;
using CityInfo_8_0_TestSetup.Setup;
using Contracts;
using Entities.Models;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
    public class SynchronousInMemoryCityRepositoryLayerTest
    {
        private DbContextOptions<DatabaseContext> _contextOptions;
        private ICityRepository _cityRepository;
        private IRepositoryWrapper _repositoryWrapper;

        public SynchronousInMemoryCityRepositoryLayerTest()
        {
            //_contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            //    .UseInMemoryDatabase("SynchronousInMemoryRepositoryTest")
            //    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            //    .Options;

            //var context = new UnitTestDatabaseContext(_contextOptions, null);

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //SynchronousSetupDatabaseData.SeedDatabaseData(context);

            //_cityRepository = new CityRepository(context);
            //_repositoryWrapper = new RepositoryWrapper(context);
        }

        //[Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
        //          // bruger InLineData !!!
        //[InlineData(false)]  // TestCase 1
        //[InlineData(true)]   // TestCase 2
        //public async Task InMemory_Test_CityRepository_GetAllCities_Using_CityRepository(bool includeRelations)
        //{
        //    // Arrange

        //    // Act
        //    IEnumerable<City> CityIEnumerable = await _cityRepository.GetAllCities(includeRelations);
        //    List<City> CityList = CityIEnumerable.ToList();

        //    // Assert
        //    SynchronousCustomAssert.InMemoryModeCheckCitiesRead(CityList, includeRelations);
        //}

        //[Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
        //          // bruger InLineData !!!
        //[InlineData(false)]  // TestCase 1
        //[InlineData(true)]   // TestCase 2
        //public async Task InMemory_Test_CityRepository_GetAllCities_Using_RepositoryWrapper(bool includeRelations)
        //{
        //    // Arrange

        //    // Act
        //    IEnumerable<City> CityIEnumerable = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(false);
        //    List<City> CityList = CityIEnumerable.ToList();

        //    // Assert
        //    SynchronousCustomAssert.InMemoryModeCheckCitiesRead(CityList, includeRelations);
        //}
    }
}
