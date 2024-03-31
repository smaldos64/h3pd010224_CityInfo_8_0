using CityInfo_8_0_TestSetup.Assertions;
using CityInfo_8_0_TestSetup.Database;
using CityInfo_8_0_TestSetup.Setup;
using CityInfo_8_0_TestSetup.ViewModels;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Repository;
using Services;
using ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
    public class MockDatabaseLayerCityRepositoryLayerTest
    {
        //private Mock<ICityRepository> _mockCityRepository;
        //private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<DatabaseContext> _mockDbContext;
        private Mock<IRepositoryBase<City>> _mockRepositoryBase;

        private DbContextOptions<DatabaseContext> _contextOptions;
        private IRepositoryWrapper _repositoryWrapper;
        private ICityRepository _cityRepository;
        //private ICityService _cityService;
        //private ICityLanguageService _cityLanguage;
        //private IPointOfInterestService _pointOfInterestService;
        private DatabaseViewModel _databaseViewModel;

        public MockDatabaseLayerCityRepositoryLayerTest()
        {
            Task.Run(async () =>
            {
                this._mockRepositoryBase = new Mock<IRepositoryBase<City>>();

                this._mockDbContext = new Mock<DatabaseContext>();
                
                this._databaseViewModel = new DatabaseViewModel();

                this._repositoryWrapper = new RepositoryWrapper(this._mockDbContext.Object);

                this._cityRepository = new CityRepository(this._mockDbContext.Object);
            }).GetAwaiter().GetResult();
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        //public async Task Mock_Test_City_GetAllCities_Using_CityService_using_Moq_Return(bool includeRelations)
        public async Task Mock_Test_CityRepository_GetAllCities_Using_CityRepository(bool includeRelations)
        {
            // Arrange
            await SetupDatabaseData.SeedDatabaseDataWithObject(null, this._databaseViewModel, includeRelations);
            this._mockRepositoryBase.Setup(func => func.FindAll()).Returns((Task<IEnumerable<City>>)await HandleDatabaseDataInMemory.FindAllCities(this._databaseViewModel,
                                                                                                                                                   includeRelations));
            //this._mockRepositoryBase.Setup(func => func.FindAll()).Returns((Task<IEnumerable<City>>)HandleDatabaseDataInMemory.GetListFromBaseClass<City>(this._databaseViewModel,
            //                                                                                                                                       includeRelations));

            // Act
            IEnumerable<City> CityIEnumerable = await this._cityRepository.GetAllCities(includeRelations);
            List<City> CityList = CityIEnumerable.ToList();

            List<City> CityListSorted = new List<City>();
            CityListSorted = CityList.OrderBy(c => c.CityId).ToList();

            // Assert
            bool CompareResult = CustomAssert.AreListOfObjectsEqualByFields<City>(CityListSorted,
                                                                                  _databaseViewModel.CityList,
                                                                                  false);
            Assert.True(CompareResult);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task Mock_Test_CityRepository_GetAllCities_Using_RepositoryWrapper(bool includeRelations)
        {
            // Arrange

            // Act
            IEnumerable<City> CityIEnumerable = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(false);
            List<City> CityList = CityIEnumerable.ToList();

            List<City> CityListSorted = new List<City>();
            CityListSorted = CityList.OrderBy(c => c.CityId).ToList();

            // Assert
            bool CompareResult = CustomAssert.AreListOfObjectsEqualByFields<City>(CityListSorted,
                                                                                  _databaseViewModel.CityList,
                                                                                  false);
            Assert.True(CompareResult);
        }
    }
}
