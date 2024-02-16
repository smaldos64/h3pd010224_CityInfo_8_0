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
    public class CityRepositoryTest4
    {
        private Mock<IRepositoryBase<City>> _mockRepository;
        private CityController _cityController;
        private Mock<DatabaseContext> _mockDatabaseContext;
        private DatabaseContext _databaseContext;

        private IRepositoryWrapper _repositoryWrapper;

        public CityRepositoryTest4()
        {
            this._mockRepository = new Mock<IRepositoryBase<City>>();

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabaseTest")
                .Options;
            this._mockDatabaseContext = new Mock<DatabaseContext>(options);
            //this._databaseContext = new DatabaseContext(options);
            //SetupDatabaseData.SeedDatabaseData(this._databaseContext);

            //List<Language> LanguageObjectList = new List<Language>()
            //{
            //    new Language
            //    {
            //        LanguageName = "dansk"
            //    },
            //    new Language
            //    {
            //        LanguageName = "engelsk"
            //    },
            //    new Language
            //    {
            //        LanguageName = "tysk"
            //    }
            //};
            //this._mockDatabaseContext.    AddRangeAsync(LanguageObjectList);
            //NumberOfDatabaseObjectsChanged = context.SaveChanges();
            //SetupDatabaseData.SeedDatabaseData(this._mockDatabaseContext);

            this._mockDatabaseContext = new Mock<DatabaseContext>(options);
            this._repositoryWrapper = new RepositoryWrapper(this._mockDatabaseContext.Object);
        }

        [Fact]
        public async void Test_CityRepository_GetAllCities_4()
        {
            // Arrange
            //IRepositoryWrapper RepositoryWrapperObject = new RepositoryWrapper(this._databaseContext);
            //IRepositoryWrapper RepositoryWrapperObject1 = new RepositoryWrapper(this._databaseContext);
            //CityRepository CityRepositoryObject = new CityRepository(this._databaseContext);
            // Act
            IEnumerable<City> CityList = await this._repositoryWrapper.CityRepositoryWrapper.GetAllCities(false);

            // Assert
            Assert.Equal(SetupDatabaseData.CityObjectList.Count, CityList.Count());
        }
    }
}
