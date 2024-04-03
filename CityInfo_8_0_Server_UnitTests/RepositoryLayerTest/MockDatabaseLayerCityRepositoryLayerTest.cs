using CityInfo_8_0_Server_UnitTests.Mock;
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
using Moq.EntityFrameworkCore;
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
        private Mock<ICityRepository> _mockCityRepository;
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
                // Forsøg herunder på Mock af DatabaseContext
                //this._cityRepository = CityRepositoryMock.GetMock();

                this._mockCityRepository = new Mock<ICityRepository>();

                this._databaseViewModel = new DatabaseViewModel();
               
                this._mockDbContext = new Mock<DatabaseContext>();

                // Alternativ måde herunder at få det til at køre på.
                this._cityRepository = new CityRepository(this._mockDbContext.Object);
                this._repositoryWrapper = new RepositoryWrapper(this._mockDbContext.Object);
            }).GetAwaiter().GetResult();
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        //[InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        //public async Task Mock_Test_City_GetAllCities_Using_CityService_using_Moq_Return(bool includeRelations)
        public async Task Mock_Test_CityRepository_GetAllCities_Using_CityRepository(bool IncludeRelations)
        {
            // Arrange
            await SetupDatabaseData.SeedDatabaseDataWithObject(null, _databaseViewModel, IncludeRelations);
            //this._cityRepository = CityRepositoryMock.GetMock();

            // Kører med IncludeRelations == true med linjen herunder.
            this._mockDbContext.Setup<DbSet<City>>(x => x.Core_8_0_Cities).ReturnsDbSet(SetupDatabaseData.GetAllCitiesAsList(this._databaseViewModel, IncludeRelations));

            // Act
            IEnumerable<City> CityIEnumerable = await this._cityRepository.GetAllCities(IncludeRelations);
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
        //[InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task Mock_Test_CityRepository_GetAllCities_Using_RepositoryWrapper(bool IncludeRelations)
        {
            // Arrange
            await SetupDatabaseData.SeedDatabaseDataWithObject(null, _databaseViewModel, IncludeRelations);

            this._mockDbContext.Setup<DbSet<City>>(x => x.Core_8_0_Cities).ReturnsDbSet(SetupDatabaseData.GetAllCitiesAsList(this._databaseViewModel, IncludeRelations));

            // Act
            IEnumerable<City> CityIEnumerable = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(IncludeRelations);
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
