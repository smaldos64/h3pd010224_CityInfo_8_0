using CityInfo_8_0_Server_IntegrationTests.Setup;
using Entities.DataTransferObjects;
using Entities.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CityInfo_8_0_Server_IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Entities;

namespace CityInfo_8_0_Server_IntegrationTests.ControllerLayerTest
{
  public class CityControllerTest : IClassFixture<TestingWebAppFactory<Program>> //, IDisposable
  {
    private readonly HttpClient _client;
    protected DatabaseContext SqlDbContext { get; }
    
    public CityControllerTest(TestingWebAppFactory<Program> factory)
    {
      _client = factory.CreateClient();
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

      return;
    }

    [Fact]
    public async Task CreateCityValidModel()
    {
      // Arrange
      string UserName = "LTPE_IntegrationTest";
      string URL = $"/City/CreateCity?UserName={UserName}";

      CityDto CityDto_Object = new CityDto()
      {
        CityName = "Storvorde",
        CityDescription = "Naboby til Gudumholm",
        CountryID = SetupDatabaseData.CountryObjectList[0].CountryID,
      };

      JsonSerializedClass<CityDto> MyJsonSerializer = new JsonSerializedClass<CityDto>(CityDto_Object, UserName);

      JsonSerializeCityDtoAndUserName JsonSerializeCityDtoAndUserName_Object =
          new JsonSerializeCityDtoAndUserName(CityDto_Object, UserName);

      var FormModel = new Dictionary<string, string>
            {
                {"CityName", CityDto_Object.CityName},
                {"CityDescription", CityDto_Object.CityDescription },
                {"CountryID", CityDto_Object.CountryID.ToString() }
            };

      HttpContent Content = new FormUrlEncodedContent(new[]
                                  {
                                            new KeyValuePair<string, string>("CityDto_Object", JsonConvert.SerializeObject(CityDto_Object))
                                            //new KeyValuePair<string, string>("UserName", "LTPE")
                                        });

      var jSonModel = new StringContent(
          System.Text.Json.JsonSerializer.Serialize(CityDto_Object),
          Encoding.UTF8,
          "application/json"
      );

      var PostRequest = new HttpRequestMessage(HttpMethod.Post, URL);
      //PostRequest.Content = Content;
      PostRequest.Content = new FormUrlEncodedContent(FormModel);

      // DCH Herunder
      //HttpContent content = new FormUrlEncodedContent(ConstructFormValues(model));

      //IEnumerable<T> entitiesBeforePost = ModelRepo.GetAll().ToList();
      //Act

      //HttpResponseMessage response = await Client.PostAsync(CrudEndpoints["Create"], content);

      // Act
      var ControllerResponse = await _client.PostAsync(URL, jSonModel);
      var ControllerResponse1 = await _client.PostAsync(URL, Content);
      //var ControllerResponse2 = await _client.SendAsync(PostRequest);
      //var ControllerResponse3 = await _client.PostAsync($"/City/CreateCity1", jSonModel);
      //var ControllerResponse4 = await _client.PostAsync(URL, jSonModel1);

      // Assert
      ControllerResponse.EnsureSuccessStatusCode();
      var ControllerResponseString = await ControllerResponse.Content.ReadAsStringAsync();
      Assert.True((int.Parse(ControllerResponseString)) > 0, "Rigtig CityID modtaget !!!");
    }

    [Fact]
    public async Task DeleteCityValidCityId()
    {
      // Arrange
      int CityId = SetupDatabaseData.CityObjectList[0].CityId;
      string UserName = "LTPE_IntegrationTest";
      string URL = $"/City/DeleteCity/{CityId}?UserName={UserName}";
      var DeleteRequest = new HttpRequestMessage(HttpMethod.Delete, URL);

      // Act
      var ControllerResponse = await _client.SendAsync(DeleteRequest);

      //List<CityDto> CityList = await ReadAllCities(false, false, false, "TestFromDelete");
      //var ControllerResponse1 = await _client.DeleteAsync(URL);

      // Assert
      ControllerResponse.EnsureSuccessStatusCode();
    }

    //public void Dispose()
    //{
    //  bool TestDB = false;
    //  TestDB = SqlDbContext.Database.EnsureDeleted();
    //  GC.SuppressFinalize(this);
    //}
  }
}
