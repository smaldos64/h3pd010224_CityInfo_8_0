using CityInfo_8_0_Server.Controllers;
using CityInfo_8_0_Server_IntegrationTests.Setup;
using CityInfo_8_0_TestSetup.Database;
using CityInfo_8_0_TestSetup.ViewModels;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using LoggerService;
using Newtonsoft.Json;
using NLog;
using Repository;
using Services;
using ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_IntegrationTests.ControllerLayerTest
{
    public class SqlCityControllerLayerTest : IClassFixture<TestingWebAppFactory<Program>>,
                                              IClassFixture<TestDatabaseFixture>
    {
        private readonly HttpClient _client;
        private TestDatabaseFixture _fixture { get; }

        private IRepositoryWrapper _repositoryWrapper;
        private ICityService _cityService;
        private ICityLanguageService _cityLanguage;
        private IPointOfInterestService _pointOfInterestService;
        private DatabaseViewModel _databaseViewModel;
        private DatabaseContext _context;
        private ILoggerManager _loggerManager;
        private DatabaseContext _databaseContext;

        public SqlCityControllerLayerTest(TestingWebAppFactory<Program> factory,
                                          TestDatabaseFixture fixture)
        {
            this._client = factory.CreateClient();
            this._fixture = fixture;

            _databaseContext = this._fixture.CreateContext();
            //_repositoryWrapper = new RepositoryWrapper(this._fixture.CreateContext());
            _repositoryWrapper = new RepositoryWrapper(_databaseContext);
            _cityLanguage = new CityLanguageService(_repositoryWrapper);
            _pointOfInterestService = new PointOfInterestService(_repositoryWrapper);
            _cityService = new CityService(_repositoryWrapper,
                                           _cityLanguage,
                                           _pointOfInterestService);

            _loggerManager = new LoggerManager();
        }

        [Theory]  // Læg mærke til at vi bruger Theory her, da vi også 
                  // bruger InLineData !!!
        [InlineData(false, false, false, "LTPE_IntegrationTest")]  // TestCase 1
        [InlineData(false, false, true, "LTPE_IntegrationTest")]   // TestCase 2
        [InlineData(false, true, false, "LTPE_IntegrationTest")]   // TestCase 3
        [InlineData(false, true, true, "LTPE_IntegrationTest")]    // TestCase 4
        [InlineData(true, false, false, "LTPE_IntegrationTest")]
        [InlineData(true, false, true, "LTPE_IntegrationTest")]
        [InlineData(true, true, false, "LTPE_IntegrationTest")]
        [InlineData(true, true, true, "LTPE_IntegrationTest")]
        public async Task ReadAllCities(bool includeRelations,
                                    bool UseLazyLoading,
                                    bool UseMapster,
                                    string UserName)
        {
            // Arrange
            string URL = $"/City/GetCities/?includeRelations={includeRelations}&UseLazyLoading={UseLazyLoading}&UseMapster={UseMapster}&UserName={UserName}";
            var Controller = new CityController(_loggerManager, 
                                                _repositoryWrapper,
                                                _cityService);

            // Act
            var ControllerResponse = await _client.GetAsync(URL);

            // Assert
            ControllerResponse.EnsureSuccessStatusCode();
            var ControllerResponseString = await ControllerResponse.Content.ReadAsStringAsync();

            List<CityDto> CityList = JsonConvert.DeserializeObject<List<CityDto>>(ControllerResponseString);
            CityList.Sort((x, y) => x.CityId.CompareTo(y.CityId));

            Assert.Equal(SetupDatabaseData.CityObjectList.Count, CityList.Count);
            if ((true == includeRelations) || (true == UseLazyLoading))
            {
                for (int Counter = 0; Counter < SetupDatabaseData.CityObjectList.Count; Counter++)
                {
                    Assert.Equal(SetupDatabaseData.CityObjectList[Counter].CityLanguages.Count,
                    CityList[Counter].CityLanguages.Count);
                }
            }
            else
            {
                for (int Counter = 0; Counter < SetupDatabaseData.CityObjectList.Count; Counter++)
                {
                    Assert.Equal(0, CityList[Counter].CityLanguages.Count);
                }

            }
        }

    }
}
