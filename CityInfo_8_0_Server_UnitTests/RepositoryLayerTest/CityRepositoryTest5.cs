using CityInfo_8_0_Server.Controllers;
using CityInfo_8_0_Server_UnitTests.Setup;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
  public  class CityRepositoryTest5
  {
    protected static DatabaseContext ConstructSqlDbContext()
    {
      DbContextOptions<DatabaseContext> options = new DbContextOptionsBuilder<DatabaseContext>()
          .UseInMemoryDatabase(Guid.NewGuid().ToString())
          .Options;
      return new DatabaseContext(options);
    }

    [Fact]
    public async Task GetAllCitiesFromBaseRepository()
    {
      //Arrange
      RepositoryBase<City> repositoryBase = new CityRepository(ConstructSqlDbContext());

      //Act
      repositoryBase.DisableLazyLoading();
      var CityList = await repositoryBase.FindAll();

      //Assert
    }
  }
}
