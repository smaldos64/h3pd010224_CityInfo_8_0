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

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
    public class CityRepositoryTest6
    {
        private readonly DbContextOptions<DatabaseContext> _contextOptions;
        //private readonly DbContextOptions<UnitTestDatabaseContext> _contextOptions;
        private readonly CityRepository _cityRepository;    

        public CityRepositoryTest6()
        {
            _contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("BloggingControllerTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
            //_contextOptions = new DbContextOptionsBuilder<UnitTestDatabaseContext>()
            //.UseInMemoryDatabase("BloggingControllerTest")
            //.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            //.Options;

            //var context = new DatabaseContext(_contextOptions, null);
            var context = new UnitTestDatabaseContext(_contextOptions, null);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            //context.AddRange(
            //new Blog { Name = "Blog1", Url = "http://blog1.com" },
            //    new Blog { Name = "Blog2", Url = "http://blog2.com" });

            //context.SaveChanges();

            SetupDatabaseData.SeedDatabaseData(context);

            _cityRepository = new CityRepository(context);
        }

        [Fact]
        public async void Test_CityRepository_GetAllCities()
        {
            // Arrange
            //var _contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            //.UseInMemoryDatabase("BloggingControllerTest")
            //.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            //.Options;

            //var context = new DatabaseContext(_contextOptions);

            //var controller = new RegistrationController(context);
            //CityRepository CityRepositoryObject = new CityRepository(context);

            // Act
            //IEnumerable<City> CityList = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(false);
            IEnumerable<City> CityList = await _cityRepository.GetAllCities(false);
            //IEnumerable<City> CityList = await CityRepositoryObject.GetAllCities(false);
            List<City> cities = CityList.ToList();

            // Assert
            Assert.Equal(3, cities.Count);
        }
    }
}
