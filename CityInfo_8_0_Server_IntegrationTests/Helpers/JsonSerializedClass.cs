using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_IntegrationTests.Helpers
{
  public class JsonSerializedClass<T> where T : class
  {
    T ?CityDtoClassType { get; set; }
    string ?UserName { get; set; }

    public JsonSerializedClass(T CityDtoClassType, string UserName)
    {
      this.CityDtoClassType = CityDtoClassType;
      this.UserName = UserName;
    }
  }
}
