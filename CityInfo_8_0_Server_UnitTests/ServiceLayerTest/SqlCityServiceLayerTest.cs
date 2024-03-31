using CityInfo_8_0_TestSetup.Assertions;
using CityInfo_8_0_TestSetup.Database;
using CityInfo_8_0_TestSetup.Setup;
using CityInfo_8_0_TestSetup.ViewModels;
using Contracts;
using Entities;
using Entities.Models;
using Repository;
using Services;
using ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_UnitTests.ServiceLayerTest
{
    public class SqlCityServiceLayerTest : IClassFixture<TestDatabaseFixture>
    {
        private TestDatabaseFixture _fixture { get; }

        private IRepositoryWrapper _repositoryWrapper;
        private ICityService _cityService;
        private ICityLanguageService _cityLanguage;
        private IPointOfInterestService _pointOfInterestService;
        private DatabaseViewModel _databaseViewModel;
        private DatabaseContext _context;

        public SqlCityServiceLayerTest(TestDatabaseFixture fixture)
        {
            this._fixture = fixture;
           
            _repositoryWrapper = new RepositoryWrapper(this._fixture.CreateContext());
            _cityLanguage = new CityLanguageService(_repositoryWrapper);
            _pointOfInterestService = new PointOfInterestService(_repositoryWrapper);
            _cityService = new CityService(_repositoryWrapper,
                                           _cityLanguage,
                                           _pointOfInterestService);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task Sql_Test_CityService_GetAllCities_Using_CityService(bool includeRelations)
        {
            // Arrange
            _repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();

            // Act
            IEnumerable<City> CityIEnumerable = await _cityService.GetAllCities(includeRelations);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            bool CompareResult = CustomAssert.AreListOfObjectsEqualByFields<City>(CityList,
                                                                                  this._fixture.DatabaseViewModelObject.CityList,
                                                                                  false);
            Assert.True(CompareResult);
        }

    }
}
