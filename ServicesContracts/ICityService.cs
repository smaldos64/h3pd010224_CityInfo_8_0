using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities.Models;

namespace ServicesContracts
{
  public interface ICityService
  {
    Task<IEnumerable<City>> GetCities(bool IncludeRelations = false);
  }
}
