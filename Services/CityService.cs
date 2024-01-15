using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using ServicesContracts;
using Entities.Models;

namespace Services
{
  public class CityService : ICityService
  {
      private readonly IRepositoryWrapper _repositoryWrapper;

      public CityService(IRepositoryWrapper repositoryWrapper)
      {
        this._repositoryWrapper = repositoryWrapper; 
      }

    public async Task<IEnumerable<City>> GetCities(bool IncludeRelations = false)
    {
      return (await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(IncludeRelations));
    }

  }
}
