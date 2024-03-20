using CityInfo_8_0_Server_UnitTests.Assertions;
using CityInfo_8_0_Server_UnitTests.Database;
using CityInfo_8_0_Server_UnitTests.Setup;
using CityInfo_8_0_Server_UnitTests.ViewModels;
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
        public TestDatabaseFixture Fixture { get; }

        private IRepositoryWrapper _repositoryWrapper;
        private ICityService _cityService;
        private ICityLanguageService _cityLanguage;
        private IPointOfInterestService _pointOfInterestService;
        private DatabaseViewModel _databaseViewModel;
        private DatabaseContext _context;

        public SqlCityServiceLayerTest(TestDatabaseFixture fixture)
        {
            this.Fixture = fixture;
           
            _repositoryWrapper = new RepositoryWrapper(this.Fixture.CreateContext());
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
            await CustomAssert.InMemoryModeCheckCitiesReadWithObject(CityList, this.Fixture.DatabaseViewModelObject, includeRelations, true);
        }

    }
}
