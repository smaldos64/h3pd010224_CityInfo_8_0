using CityInfo_8_0_Server_UnitTests.Setup;
using CityInf0_8_0_Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CityInfo_8_0_Server_UnitTests.RepositoryLayerTest
{
    public class CityRepositoryTest : IClassFixture<TestingWebAppFactory<Program>>
    {
        private readonly HttpClient _client;
        public CityRepositoryTest(TestingWebAppFactory<Program> factory) 
        {
            this._client = factory.CreateClient();
        } 
    }
}
