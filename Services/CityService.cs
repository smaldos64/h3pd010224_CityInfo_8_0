using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Contracts;
using ServicesContracts;
using Entities.Models;
using Entities.DataTransferObjects;
//using Microsoft.AspNetCore.Http; // StatusCodes

using Mapster;

namespace Services
{
  public class CityService : ICityService
  {
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ICityLanguage _cityLanguage;

    public CityService(IRepositoryWrapper repositoryWrapper,
                       ICityLanguage cityLanguage)
    {
      this._repositoryWrapper = repositoryWrapper;
      this._cityLanguage = cityLanguage;
      UtilityService.SetupMapsterConfiguration();
    }

    public async Task<IEnumerable<City>> GetCities(bool IncludeRelations = false)
    {
      return (await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(IncludeRelations));
    }

    public async Task<int> SaveCity(City City_Object)
    {
      int NumberOfObjectsChanged;

      try
      {
        await _repositoryWrapper.CityRepositoryWrapper.Create(City_Object);
        NumberOfObjectsChanged = await _repositoryWrapper.CityRepositoryWrapper.Save();

        return (NumberOfObjectsChanged);
      }
      catch (Exception Error)
      {
        return (0);
      }
    }

    public async Task<int> SaveCityAllInfo(City City_Object)
    {
      int NumberOfObjectsChanged;

      try
      {
        await _repositoryWrapper.CityRepositoryWrapper.Create(City_Object);
        NumberOfObjectsChanged = await _repositoryWrapper.CityRepositoryWrapper.Save();

        return (NumberOfObjectsChanged);
      }
      catch (Exception Error)
      {
        return (0);
      }
    }

    public async Task<ICommunicationResults> UpdateCityWithAllRelations(CityForUpdateDto CityForUpdateDto_Object,
                                                List<PointOfInterestForUpdateDto> PointOfInterestForUpdateDto_List,
                                                List<CityLanguageForSaveAndUpdateDto> CityLanguageForSaveAndUpdateDto_List,
                                                string UserName,
                                                bool DeleteOldElementsInListsNotSpecifiedInCurrentLists = true)
    {
      int NumberOfObjectsChanged = 0;
      //int ListCounter = 0;
      List<int> AddedList = new List<int>();
      ICommunicationResults CommunicationResults_Object = new CommunicationResults(true);

      City CityFromRepo = await _repositoryWrapper.CityRepositoryWrapper.FindOne(CityForUpdateDto_Object.CityId);

      if (null == CityFromRepo)
      {
        CommunicationResults_Object.ResultString = $"City With ID : {CityForUpdateDto_Object.CityId} not found in Cities for {UserName} in action UpdateCityWithAllRelations";
        CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotFound;
        return (CommunicationResults_Object);
      }

      TypeAdapter.Adapt(CityForUpdateDto_Object, CityFromRepo);
      await _repositoryWrapper.CityRepositoryWrapper.Update(CityFromRepo);
      NumberOfObjectsChanged = await _repositoryWrapper.CityRepositoryWrapper.Save();
      if (1 != NumberOfObjectsChanged)
      {
        CommunicationResults_Object.ResultString = $"City with Id : {CityForUpdateDto_Object.CityId} not updated for {UserName} in action UpdateCityWithAllRelations";
        CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotModified;
        return (CommunicationResults_Object);
      }

      if (null != PointOfInterestForUpdateDto_List)
      {
        for (int Counter = 0; Counter < PointOfInterestForUpdateDto_List.Count; Counter++)
        {
          if (0 == PointOfInterestForUpdateDto_List[Counter].PointOfInterestId)
          {
            PointOfInterest PointOfInterest_Object =
                PointOfInterestForUpdateDto_List[Counter].Adapt<PointOfInterest>();

            await _repositoryWrapper.PointOfInterestRepositoryWrapper.Create(PointOfInterest_Object);
            NumberOfObjectsChanged = await _repositoryWrapper.CityRepositoryWrapper.Save();

            if (1 != NumberOfObjectsChanged)
            {
              CommunicationResults_Object.ResultString = $"PointOfInterest Object with Name : {PointOfInterest_Object.PointOfInterestName} not created for {UserName} in action UpdateCityWithAllRelations";
              CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotModified;
              return (CommunicationResults_Object);
            }
            AddedList.Add(PointOfInterest_Object.PointOfInterestId);
          }
          else
          {
            PointOfInterest PointOfInterestFromRepo = 
              await _repositoryWrapper.PointOfInterestRepositoryWrapper.FindOne(PointOfInterestForUpdateDto_List[Counter].PointOfInterestId);
        
            if (null == PointOfInterestFromRepo)
            {
              CommunicationResults_Object.ResultString = $"PointOfInterest Object with PointOfInterestId : {PointOfInterestForUpdateDto_List[Counter].PointOfInterestId} not found in Database for {UserName} in action UpdateCityWithAllRelations";
              CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotFound;
              return (CommunicationResults_Object);
            }

            TypeAdapter.Adapt(PointOfInterestForUpdateDto_List[Counter], PointOfInterestFromRepo);
            await _repositoryWrapper.PointOfInterestRepositoryWrapper.Update(PointOfInterestFromRepo);
            NumberOfObjectsChanged = await _repositoryWrapper.CityRepositoryWrapper.Save();

            if (1 != NumberOfObjectsChanged)
            {
              CommunicationResults_Object.ResultString = $"PointOfInterest Object with PointOfInterestId : {PointOfInterestFromRepo.PointOfInterestId} not updated for {UserName} in action UpdateCityWithAllRelations";
              CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotModified;
              return (CommunicationResults_Object);
            }
          }
        }

        if (true == DeleteOldElementsInListsNotSpecifiedInCurrentLists)
        {
          var PointOfInterestList = await _repositoryWrapper.PointOfInterestRepositoryWrapper.GetAllPointOfInterestWithCityID(CityForUpdateDto_Object.CityId, false);
         
          foreach (PointOfInterest PointOfInterest_object in PointOfInterestList)
          {
            var Matches = PointOfInterestForUpdateDto_List.Where(p => p.PointOfInterestId == PointOfInterest_object.PointOfInterestId);
            if (0 == Matches.Count())
            {
              var Matches1 = AddedList.Any(p => p == PointOfInterest_object.PointOfInterestId);

              if (!Matches1)
              {
                // Et af de nuværende PointOfinterests for det angivne CityId
                // findes ikke i den nye liste over ønskede opdateringer og heller
                // ikke i liste for nye PointOfInterests for det angivne CityId. 
                // Og desuden er parameteren for at slette "gamle" elementer i
                // PointOfInterest listen for det angivne CityId sat. Så slet 
                // dette PointOfInterest fra databasen !!!

                PointOfInterest PointOfInterestFromRepo = await _repositoryWrapper.PointOfInterestRepositoryWrapper.FindOne(PointOfInterest_object.PointOfInterestId);

                if (null == PointOfInterestFromRepo)
                {
                  CommunicationResults_Object.ResultString = $"PointOfInterest Object with PointOfInterestId : {PointOfInterest_object.PointOfInterestId} not found for delete for {UserName} in action UpdateCityWithAllRelations";
                  CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotModified;
                  return (CommunicationResults_Object);
                }

                await _repositoryWrapper.PointOfInterestRepositoryWrapper.Delete(PointOfInterestFromRepo);
                NumberOfObjectsChanged = await _repositoryWrapper.PointOfInterestRepositoryWrapper.Save();

                if (1 != NumberOfObjectsChanged)
                {
                  CommunicationResults_Object.ResultString = $"PointOfInterest Object with PointOfInterestId : {PointOfInterest_object.PointOfInterestId} not deleted for {UserName} in action UpdateCityWithAllRelations";
                  CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotModified;
                  return (CommunicationResults_Object);
                }
              }
            }
          }
        }
      }

      if (null != CityLanguageForSaveAndUpdateDto_List)
      {
        CommunicationResults_Object = await this._cityLanguage.UpdateCityLanguagesList(CityLanguageForSaveAndUpdateDto_List,
                                                                                       DeleteOldElementsInListsNotSpecifiedInCurrentLists,
                                                                                       UserName);
        if (true == CommunicationResults_Object.HasErrorOccured)
        {
          return CommunicationResults_Object;
        }
      }

      CommunicationResults_Object.ResultString = $"City and all relations has been updated/saved for {UserName} in action UpdateCityWithAllRelations";
      CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.OK;
      CommunicationResults_Object.HasErrorOccured = false;
      return (CommunicationResults_Object);
    }
  }
}
