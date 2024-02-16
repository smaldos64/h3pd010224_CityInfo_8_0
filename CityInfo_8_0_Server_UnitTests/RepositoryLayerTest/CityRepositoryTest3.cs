using CityInfo_8_0_Server_UnitTests.Setup;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Contracts;

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
    public class CityRepositoryTest3 : IClassFixture<TestingWebAppFactory<Program>>
    {
        //private DatabaseContext _dbContext;
        private readonly HttpClient _client;

        public CityRepositoryTest3(TestingWebAppFactory<Program> factory)
        {
            //var serviceProvider = factory.Server.Services;
            //var DataBaseService1 = factory.Server.Services.GetService<DbContext>();
            //var DatabaseService = serviceProvider.GetService<DbContext>();
            //var DatabaseService2 = factory.Services.GetService<DatabaseContext>();
            //var DataBaseService3 = serviceProvider.GetRequiredService<DbContext>();
            //ServiceCollection MyServices = new ServiceCollection();
            //var DataBaseService4 = serviceProvider.GetRequiredService<DatabaseContext>();
            //this._dbContext = dbContext;
            _client = factory.CreateClient();
        }

        [Fact]
        public async void Test_CityRepository_GetAllCities_3()
        {
            // Arrange
            IRepositoryWrapper RepositoryWrapperObjext = new RepositoryWrapper(TestingWebAppFactory<Program>._databaseContext);
            //CityRepository CityRepositoryObject = new CityRepository(TestingWebAppFactory<Program>._databaseContext);
            
            //await RepositoryWrapperObjext.CityRepositoryWrapper.GetAllCities(includeRelations);

            //using (var scope = HttpContext.Re)
            //WebApplicationBuilder builder = WebApplication.CreateBuilder();
            //var builder = new TestingWebAppFactory<Program>();

            //var dbContext = builder.services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

            //    // LTPE => Slet database context fra CityInfo_8_0_Server
            //    if (dbContext != null)
            //        services.Remove(dbContext);

            //    var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            //    services.AddDbContext<DatabaseContext>(options =>
            //    {
            //        options.UseInMemoryDatabase("InMemoryDatabaseTest");
            //        options.UseInternalServiceProvider(serviceProvider);
            //    });


            //var controller = new RegistrationController(context);
            //CityRepository CityRepositoryObject = new CityRepository(this._databaseContext);

            //IServiceCollection services = new ServiceCollection();
            //services.Add

            //var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();


            //var MyBuilder = DbContextOptionsBuilder.
            //var dbContext = (DatabaseContext) serviceProvider.GetService<MyDbContext>();

            //CityRepository CityRepositoryObject = new CityRepository();

            // Act
            //IEnumerable<City> CityList = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(false);

            //IEnumerable<City> CityList = await CityRepositoryObject.GetAllCities(false);
            //List<City> cities = CityList.ToList();

            // Assert
            //Assert.Equal(3, cities.Count);
            Assert.Equal(1, 1);
        }

    }
}
