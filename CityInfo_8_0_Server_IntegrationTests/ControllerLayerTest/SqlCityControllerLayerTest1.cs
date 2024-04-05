using CityInfo_8_0_TestSetup.Database;
using CityInfo_8_0_TestSetup.Assertions;
using Entities.Models;
using Entities;
using LoggerService;
using NLog.Config;
using Repository;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityInfo_8_0_TestSetup.ViewModels;
using Contracts;
using ServicesContracts;
using CityInfo_8_0_Server.Controllers;
using Entities.DataTransferObjects;
using Newtonsoft.Json;
using CityInfo_8_0_Server_IntegrationTests.Setup;
using System.Net;
using Mapster;
using CityInfo_8_0_TestSetup.Setup;
using CityInfo_8_0_Server.ViewModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CityInfo_8_0_Server_IntegrationTests.ControllerLayerTest
{
    public class SqlCityControllerLayerTest1 : IClassFixture<TestDatabaseFixture>
    {
        private TestDatabaseFixture _fixture { get; }

        private IRepositoryWrapper _repositoryWrapper;
        private ICityService _cityService;
        private ICityLanguageService _cityLanguage;
        private IPointOfInterestService _pointOfInterestService;
        private DatabaseViewModel _databaseViewModel;
        private DatabaseContext _context;
        private ILoggerManager _loggerManager;
        private DatabaseContext _databaseContext;

        private CityEducationController _cityController;

        public SqlCityControllerLayerTest1(TestDatabaseFixture fixture)
        {
            // Kode i constructoren her bliver kaldt for hver TestCase.

            this._fixture = fixture;

            _databaseContext = this._fixture.CreateContext();
            _repositoryWrapper = new RepositoryWrapper(_databaseContext);
            _cityLanguage = new CityLanguageService(_repositoryWrapper);
            _pointOfInterestService = new PointOfInterestService(_repositoryWrapper);
            _cityService = new CityService(_repositoryWrapper,
                                           _cityLanguage,
                                           _pointOfInterestService);

            _loggerManager = new LoggerManager();

            _cityController = new CityEducationController(_loggerManager,
                                                 _repositoryWrapper,
                                                 _cityService);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false, false, false, MyConst.IntegrationTestUserName)]  // TestCase 1
        [InlineData(false, false, true, MyConst.IntegrationTestUserName)]   // TestCase 2
        [InlineData(false, true, false, MyConst.IntegrationTestUserName)]   // TestCase 3
        [InlineData(false, true, true, MyConst.IntegrationTestUserName)]    // TestCase 4
        [InlineData(true, false, false, MyConst.IntegrationTestUserName)]   // TestCase 5
        [InlineData(true, false, true, MyConst.IntegrationTestUserName)]    // TestCase 6
        [InlineData(true, true, false, MyConst.IntegrationTestUserName)]    // TestCase 7
        [InlineData(true, true, true, MyConst.IntegrationTestUserName)]     // TestCase 8
        public async Task ReadAllCities(bool IncludeRelations,
                                        bool UseLazyLoading,
                                        bool UseMapster,
                                        string UserName)
        {
            // Arrange
            
            // Act
            var Result = await _cityController.GetCities(IncludeRelations,
                                                         UseLazyLoading,
                                                         UseMapster,
                                                         UserName);

            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.OK);
                    
            List<CityDto> CityDtoList = (List<CityDto>)((Microsoft.AspNetCore.Mvc.ObjectResult)Result).Value;

            List<City> CityList = new List<City>();

            CityList = CityDtoList.Adapt<City[]>().ToList();

            // Assert
            //await CustomAssert.InMemoryModeCheckCitiesReadWithObject(CityList, this._fixture.DatabaseViewModelObject, IncludeRelations || UseLazyLoading, true);
            bool CompareResult = CustomAssert.AreListOfObjectsEqualByFields<City>(CityList,
                                                                                  this._fixture.DatabaseViewModelObject.CityList,
                                                                                  false);
            Assert.True(CompareResult);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false, false, false, MyConst.IntegrationTestUserName)]  // TestCase 1
        [InlineData(false, false, true, MyConst.IntegrationTestUserName)]   // TestCase 2
        [InlineData(false, true, false, MyConst.IntegrationTestUserName)]   // TestCase 3
        [InlineData(false, true, true, MyConst.IntegrationTestUserName)]    // TestCase 4
        [InlineData(true, false, false, MyConst.IntegrationTestUserName)]   // TestCase 5
        [InlineData(true, false, true, MyConst.IntegrationTestUserName)]    // TestCase 6
        [InlineData(true, true, false, MyConst.IntegrationTestUserName)]    // TestCase 7
        [InlineData(true, true, true, MyConst.IntegrationTestUserName)]     // TestCase 8
        public async Task ReadAllCitiesUsingServiceLayer(bool IncludeRelations,
                                                         bool UseLazyLoading,
                                                         bool UseMapster,
                                                         string UserName)
        {
            // Arrange

            // Act
            var Result = await _cityController.GetCitiesServiceLayer(IncludeRelations,
                                                                    UseLazyLoading,
                                                                    UseMapster,
                                                                    UserName);

            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.OK);
           
            List<CityDto> CityDtoList = (List<CityDto>)((Microsoft.AspNetCore.Mvc.ObjectResult)Result).Value;

            List<City> CityList = new List<City>();
            CityList = CityDtoList.Adapt<City[]>().ToList();

            // Assert
            //await CustomAssert.InMemoryModeCheckCitiesReadWithObject(CityList, this._fixture.DatabaseViewModelObject, IncludeRelations || UseLazyLoading, true);

            bool CompareResult = CustomAssert.AreListOfObjectsEqualByFields<City>(CityList,
                                                                                      this._fixture.DatabaseViewModelObject.CityList,
                                                                                      false);
            Assert.True(CompareResult);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false, false, 2, MyConst.IntegrationTestUserName)]  // TestCase 1
        [InlineData(false, true, 2, MyConst.IntegrationTestUserName)]   // TestCase 2
        [InlineData(true, false, 2, MyConst.IntegrationTestUserName)]   // TestCase 3
        [InlineData(true, true, 2, MyConst.IntegrationTestUserName)]    // TestCase 4
        [InlineData(false, false, 3, MyConst.IntegrationTestUserName)]  // TestCase 5
        [InlineData(false, true, 3, MyConst.IntegrationTestUserName)]   // TestCase 6
        [InlineData(true, false, 3, MyConst.IntegrationTestUserName)]   // TestCase 7
        [InlineData(true, true, 3, MyConst.IntegrationTestUserName)]    // TestCase 8
        public async Task ReadSpecifiedNumberOfCities(bool IncludeRelations,
                                                      bool UseIQueryable,
                                                      int NumberOfCitiesToRead,
                                                      string UserName)
        {
            // Arrange

            // Act
            var Result = await _cityController.GetSpecifiedNumberOfCities(IncludeRelations,
                                                                          UseIQueryable,
                                                                          NumberOfCitiesToRead,
                                                                          UserName);

            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.OK);
   
            List<CityDto> CityDtoList = (List<CityDto>)((Microsoft.AspNetCore.Mvc.ObjectResult)Result).Value;

            List<City> CityList = new List<City>();
            CityList = CityDtoList.Adapt<City[]>().ToList();

            // Assert
            bool CompareResult = CustomAssert.AreListOfObjectsEqualByFields<City>(CityList,
                                                                                  this._fixture.DatabaseViewModelObject.CityList,
                                                                                  false);
            Assert.True(CompareResult);
        }

        [Fact]
        public async Task ReadCityWhenCityDoesNotExists()
        {
            // Arrange

            // Act
            var Result = await _cityController.GetCity(0,
                                                      false,
                                                      MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(false, MyConst.IntegrationTestUserName)]  // TestCase 1
        [InlineData(true, MyConst.IntegrationTestUserName)]   // TestCase 2
        public async Task ReadCityWhenCityExists(bool UseLazyLoading,
                                                 string UserName)
        {
            // Arrange
          
            // Act
            var Result = await _cityController.GetCity(this._fixture.DatabaseViewModelObject.CityList[0].CityId,
                                                      UseLazyLoading,
                                                      UserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.OK);
     
            CityDto CityDtoObject = (CityDto)((Microsoft.AspNetCore.Mvc.ObjectResult)Result).Value;

            City CityObject = CityDtoObject.Adapt<City>();

            int IndexInList = this._fixture.DatabaseViewModelObject.CityList.FindIndex(c => c.CityId == CityObject.CityId);
            Assert.NotEqual(-1, IndexInList);

            bool CompareResult = CustomAssert.AreObjectsEqualByFields<City>(CityObject,
                                                                            this._fixture.DatabaseViewModelObject.CityList[IndexInList],
                                                                            false);
            Assert.True(CompareResult);
        }

        private CityDto SetupCityDtoForSaveOrUpdate(bool ForUpdate = false, 
                                                    bool CityDescriptionEqualsCityName = false)
        {
            CityDto CityDtoObject = new CityDto();
            CityDtoObject.CityName = "Storvorde";
            CityDtoObject.CityDescription = "Naboby til Gudumholm";
            CityDtoObject.CountryID = this._fixture.DatabaseViewModelObject.CityList[0].CountryID;

            if (true == ForUpdate)
            {
                CityDtoObject.CityId = this._fixture.DatabaseViewModelObject.CityList[0].CityId;
            }

            if (true ==  CityDescriptionEqualsCityName) 
            {
                CityDtoObject.CityDescription = CityDtoObject.CityName;
            }

            return (CityDtoObject);
        }

        [Fact]
        public async Task SaveCityWhenCityNameEqualCityDescription()
        {
            // Arrange
            CityDto CityDtoObject = SetupCityDtoForSaveOrUpdate(CityDescriptionEqualsCityName: true);
            
            // Act
            var Result = await _cityController.CreateCity(CityDtoObject, MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SaveCityWhenParametersAreOk()
        {
            // Arrange
            CityDto CityDtoObject = SetupCityDtoForSaveOrUpdate();
            
            // Act
            var Result = await _cityController.CreateCity(CityDtoObject, MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.OK);
            int CityId = (int)((Microsoft.AspNetCore.Mvc.ObjectResult)Result).Value;

            Assert.True(CityId > 0);

            City CityObject = CityDtoObject.Adapt<City>();
            HandleDatabaseDataInMemory.AddCityToDatabaseDataInMemory(this._fixture.DatabaseViewModelObject, CityObject);
        }

        [Fact]
        public async Task UpdateCityWhenCityNameEqualCityDescription()
        {
            // Arrange
            CityDto CityDtoObject = SetupCityDtoForSaveOrUpdate(ForUpdate: true, CityDescriptionEqualsCityName: true);
            
            // Act
            var Result = await _cityController.UpdateCity(CityDtoObject.CityId,
                                                          CityDtoObject, 
                                                          MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateCityWhenCityIdDoesNotMatchCityIdInObject()
        {
            // Arrange
            CityDto CityDtoObject = SetupCityDtoForSaveOrUpdate(ForUpdate: true);

            // Act
            var Result = await _cityController.UpdateCity(CityDtoObject.CityId + 1,
                                                          CityDtoObject,
                                                          MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateCityWhenCityIdIsNotFound()
        {
            // Arrange
            CityDto CityDtoObject = SetupCityDtoForSaveOrUpdate();

            // Act
            var Result = await _cityController.UpdateCity(CityDtoObject.CityId,
                                                          CityDtoObject,
                                                          MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateCityWhenParametersAreOk()
        {
            // Arrange
            CityDto CityDtoObject = SetupCityDtoForSaveOrUpdate(ForUpdate: true);

            // Act
            var Result = await _cityController.UpdateCity(CityDtoObject.CityId,
                                                          CityDtoObject,
                                                          MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.OK);

            Assert.Contains(CityDtoObject.CityId.ToString(), (string)((Microsoft.AspNetCore.Mvc.ObjectResult)Result).Value);

            City CityObject = CityDtoObject.Adapt<City>();
            HandleDatabaseDataInMemory.UpdateCityInDatabaseDataInMemory(this._fixture.DatabaseViewModelObject, CityObject);
        }

        [Fact]
        public async Task UpdateCityWithAllRelationsWhenCityNameEqualCityDescription()
        {
            // Arrange
            CityDto CityDtoObject = SetupCityDtoForSaveOrUpdate(ForUpdate: true, CityDescriptionEqualsCityName: true);
            UpdateCityWithAllRelations UpdateCityWithAllRelations_Object = new UpdateCityWithAllRelations();
            UpdateCityWithAllRelations_Object.CityDto_Object = CityDtoObject;

            // Act
            var Result = await _cityController.UpdateCityWithAllRelations(UpdateCityWithAllRelations_Object.CityDto_Object.CityId,
                                                                           UpdateCityWithAllRelations_Object,
                                                                           false,
                                                                           false,
                                                                           MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateCityWithAllRelationsWhenCityIdDoesNotMatchCityIdInObject()
        {
            // Arrange
            CityDto CityDtoObject = SetupCityDtoForSaveOrUpdate(ForUpdate: true);
            UpdateCityWithAllRelations UpdateCityWithAllRelations_Object = new UpdateCityWithAllRelations();
            UpdateCityWithAllRelations_Object.CityDto_Object = CityDtoObject;

            // Act
            var Result = await _cityController.UpdateCityWithAllRelations(UpdateCityWithAllRelations_Object.CityDto_Object.CityId + 1,
                                                                          UpdateCityWithAllRelations_Object,
                                                                           false,
                                                                          false,
                                                                          MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateCityWithAllRelationsWhenCityIdIsNotFound()
        {
            // Arrange
            CityDto CityDtoObject = SetupCityDtoForSaveOrUpdate();
            UpdateCityWithAllRelations UpdateCityWithAllRelations_Object = new UpdateCityWithAllRelations();
            UpdateCityWithAllRelations_Object.CityDto_Object = CityDtoObject;

            // Act
            var Result = await _cityController.UpdateCityWithAllRelations(UpdateCityWithAllRelations_Object.CityDto_Object.CityId,
                                                                          UpdateCityWithAllRelations_Object,
                                                                          false,
                                                                          false,
                                                                          MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(false, MyConst.IntegrationTestUserName)]  // TestCase 1
        [InlineData(true, MyConst.IntegrationTestUserName)]   // TestCase 2
        public async Task UpdateCityWithAllRelationsWhenParametersAreOk(bool DeleteOldElementsInListsNotSpecifiedInCurrentLists,
                                                                        string UserName)
        {
            // Arrange
            CityDto CityDtoObject = SetupCityDtoForSaveOrUpdate(ForUpdate: true);
            
            UpdateCityWithAllRelations UpdateCityWithAllRelations_Object = new UpdateCityWithAllRelations();
            UpdateCityWithAllRelations_Object.CityDto_Object = CityDtoObject;
            UpdateCityWithAllRelations_Object.PointOfInterests =
            [
                new PointOfInterestForUpdateDto
                {
                    CityId = this._fixture.DatabaseViewModelObject.CityList[0].CityId,
                    PointOfInterestName = "From IntegrationTest Name",
                    PointOfInterestDescription = "From IntegrationTest Description",
                },
                new PointOfInterestForUpdateDto
                {
                    PointOfInterestId = this._fixture.DatabaseViewModelObject.PointOfInterestList[0].PointOfInterestId,
                    CityId = this._fixture.DatabaseViewModelObject.CityList[0].CityId,
                    PointOfInterestName = "From IntegrationTest Name 1",
                    PointOfInterestDescription = "From IntegrationTest Description 1",
                }
            ];

            // Act
            var Result = await _cityController.UpdateCityWithAllRelations(UpdateCityWithAllRelations_Object.CityDto_Object.CityId,
                                                                          UpdateCityWithAllRelations_Object,
                                                                          DeleteOldElementsInListsNotSpecifiedInCurrentLists,
                                                                          false,
                                                                          UserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.OK);

            City CityObject = CityDtoObject.Adapt<City>();
            HandleDatabaseDataInMemory.UpdateCityInDatabaseDataInMemory(this._fixture.DatabaseViewModelObject, CityObject, DeleteOldElementsInListsNotSpecifiedInCurrentLists);
        }

        [Fact]
        public async Task DeleteCityWhenCityIdIsNotFound()
        {
            // Arrange
            
            // Act
            var Result = await _cityController.DeleteCity(0,
                                                          MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCityWhenParametersAreOk()
        {
            // Arrange

            // Act
            var Result = await _cityController.DeleteCity(this._fixture.DatabaseViewModelObject.CityList[0].CityId,
                                                          MyConst.IntegrationTestUserName);

            // Assert
            Assert.Equal(((IStatusCodeActionResult)Result).StatusCode, (int)HttpStatusCode.OK);
      
            Assert.Contains(this._fixture.DatabaseViewModelObject.CityList[0].CityId.ToString(), (string)((Microsoft.AspNetCore.Mvc.ObjectResult)Result).Value);
           
            HandleDatabaseDataInMemory.DeleteCityInDatabaseDataInMemory(this._fixture.DatabaseViewModelObject,
                                                                        this._fixture.DatabaseViewModelObject.CityList[0].CityId);
        }
    }
}
