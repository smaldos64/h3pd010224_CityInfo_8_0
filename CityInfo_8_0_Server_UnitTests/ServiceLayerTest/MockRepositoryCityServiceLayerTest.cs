using CityInfo_8_0_Server_UnitTests.Assertions;
using CityInfo_8_0_Server_UnitTests.ViewModels;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_UnitTests.ServiceLayerTest
{
    public class MockRepositoryCityServiceLayerTest
    {

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task InMemory_Test_CityService_GetAllCities_Using_CityService(bool includeRelations)
        {
            // Arrange
            //_repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();

            //// Act
            //IEnumerable<City> CityIEnumerable = await _cityService.GetAllCities(includeRelations);
            //List<City> CityList = CityIEnumerable.ToList();

            //// Assert
            //await CustomAssert.InMemoryModeCheckCitiesReadWithObject(CityList, _databaseViewModel, includeRelations);
        }
    }
}
