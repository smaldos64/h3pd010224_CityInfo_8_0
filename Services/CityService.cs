using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using ServicesContracts;
using Entities.Models;
using Entities.DataTransferObjects;

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

    public async Task<int> SaveCity(City City_Object)
    {
      int NumberOfObjectsSaved;

      try
      {
        await _repositoryWrapper.CityRepositoryWrapper.Create(City_Object);
        NumberOfObjectsSaved = await _repositoryWrapper.CityRepositoryWrapper.Save();

        return (NumberOfObjectsSaved);
      }
      catch (Exception Error)
      {
        return (0);
      }
    }


  }
}
