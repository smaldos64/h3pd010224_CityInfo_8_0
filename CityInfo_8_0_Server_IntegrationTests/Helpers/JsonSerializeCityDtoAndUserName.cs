using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_IntegrationTests.Helpers
{
  public class JsonSerializeCityDtoAndUserName
  {
        public CityDto CityDto_Object { get; set; }
        public string UserName { get; set; }

        public JsonSerializeCityDtoAndUserName(CityDto CityDto_Object, string UserName)
        {
            this.CityDto_Object = new CityDto();
            this.UserName = UserName;
        }
    }
}
