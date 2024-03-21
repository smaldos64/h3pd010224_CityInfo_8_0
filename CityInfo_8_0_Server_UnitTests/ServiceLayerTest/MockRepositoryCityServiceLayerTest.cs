﻿using CityInfo_8_0_TestSetup.Assertions;
using CityInfo_8_0_TestSetup.ViewModels;
using CityInfo_8_0_TestSetup.Setup;
using Entities.Models;
using ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services;

namespace CityInfo_8_0_Server_UnitTests.ServiceLayerTest
{
    public class MockRepositoryCityServiceLayerTest
    {
        private Mock<ICityService> _mockCityService;
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;

        private DbContextOptions<DatabaseContext> _contextOptions;
        private IRepositoryWrapper _repositoryWrapper;
        private ICityService _cityService;
        private ICityLanguageService _cityLanguage;
        private IPointOfInterestService _pointOfInterestService;
        private DatabaseViewModel _databaseViewModel;

        public MockRepositoryCityServiceLayerTest()
        {
            Task.Run(async () =>
            {
                this._mockCityService = new Mock<ICityService>();
                this._mockRepositoryWrapper = new Mock<IRepositoryWrapper>();

                _databaseViewModel = new DatabaseViewModel();
                //await SetupDatabaseData.SeedDatabaseDataWithObject(null, _databaseViewModel);

                _cityLanguage = new CityLanguageService(_repositoryWrapper);
                _pointOfInterestService = new PointOfInterestService(_repositoryWrapper);
                _cityService = new CityService(this._mockRepositoryWrapper.Object,
                                               _cityLanguage,
                                               _pointOfInterestService);
            }).GetAwaiter().GetResult();
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task InMemory_Test_CityService_GetAllCities_Using_CityService_using_Moq_Return(bool includeRelations)
        {
            // Arrange
            await SetupDatabaseData.SeedDatabaseDataWithObject(null, _databaseViewModel, includeRelations);
            this._mockRepositoryWrapper.Setup(func => func.CityRepositoryWrapper.GetAllCities(includeRelations)).Returns(SetupDatabaseData.GetAllCities(this._databaseViewModel, includeRelations));
           
            // Act
            IEnumerable<City> CityIEnumerable = await _cityService.GetAllCities(includeRelations);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            await CustomAssert.InMemoryModeCheckCitiesReadWithObject(CityList, _databaseViewModel, includeRelations);
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false)]  // TestCase 1
        [InlineData(true)]   // TestCase 2
        public async Task InMemory_Test_CityService_GetAllCities_Using_CityService_using_Moq_Callback(bool includeRelations)
        {
            // Arrange
            await SetupDatabaseData.SeedDatabaseDataWithObject(null, _databaseViewModel, includeRelations);
            this._mockRepositoryWrapper.Setup(func => func.CityRepositoryWrapper.GetAllCities(includeRelations)).Callback(() => { }).Returns(SetupDatabaseData.GetAllCities(this._databaseViewModel, includeRelations));

            // Act
            IEnumerable<City> CityIEnumerable = await _cityService.GetAllCities(includeRelations);
            List<City> CityList = CityIEnumerable.ToList();

            // Assert
            await CustomAssert.InMemoryModeCheckCitiesReadWithObject(CityList, _databaseViewModel, includeRelations);
        }
    }
}
