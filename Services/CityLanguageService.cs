﻿using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Mapster;
using ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Services
{
  public class CityLanguageService : ICityLanguageService
  {
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CityLanguageService(IRepositoryWrapper repositoryWrapper)
    {
      this._repositoryWrapper = repositoryWrapper;
      UtilityService.SetupMapsterConfiguration();
    }

    public async Task<ICommunicationResults> UpdateCityLanguagesList(List<CityLanguageForSaveAndUpdateDto> CityLanguageForSaveAndUpdateDto_List,
                                                                     bool DeleteOldElementsInListNotSpecifiedInCurrentList,
                                                                     string UserName = "No Name",
                                                                     bool UseExtendedDatabaseDebugging = false)
    {
      int NumberOfObjectsChanged = 0;
      int NumberOfObjectsActuallySaved = 0;
      List<int> CurrentLanguageIds = new List<int>();
      ICommunicationResults CommunicationResults_Object = new CommunicationResults(true);

      if (CityLanguageForSaveAndUpdateDto_List.Count > 0)
      {
        int ListCounter;
        int CityIdSave = CityLanguageForSaveAndUpdateDto_List[0].CityId;

        for (ListCounter = 1; ListCounter < CityLanguageForSaveAndUpdateDto_List.Count; ListCounter++)
        {
          if (CityLanguageForSaveAndUpdateDto_List[ListCounter].CityId != CityLanguageForSaveAndUpdateDto_List[0].CityId)
          {
            CommunicationResults_Object.ResultString = $"CityId givet i { (ListCounter + 1).ToString()} .element er forskellig fra det første CityId element i listen. Alle CityId felter skal være ens !!!";
            CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.BadRequest;
            return (CommunicationResults_Object);
          }
        }

        IEnumerable<CityLanguage> CityLangualeListFromRepo = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetAllLanguagesFromCityId(CityLanguageForSaveAndUpdateDto_List[0].CityId);
        
        if (true == DeleteOldElementsInListNotSpecifiedInCurrentList)
        {
          foreach (var CityLanguageCombination in CityLangualeListFromRepo)
          {
            if (!_repositoryWrapper.CityLanguageRepositoryWrapper.LanguageIdFoundInCityLanguageList(CityLanguageForSaveAndUpdateDto_List,
                                                                                                    CityLanguageCombination.LanguageId))
            {
              await _repositoryWrapper.CityLanguageRepositoryWrapper.Delete(CityLanguageCombination);
              
              if (UseExtendedDatabaseDebugging) 
              {
                NumberOfObjectsChanged = await _repositoryWrapper.CityLanguageRepositoryWrapper.Save();

                if (1 != NumberOfObjectsChanged)
                {
                  CommunicationResults_Object.ResultString = $"CityLanguage Object with CityId : {CityLanguageCombination.CityId} and LanguageId : {CityLanguageCombination.LanguageId} not deleted for {UserName} in action UpdateCityLanguagesList";
                  CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotModified;
                  return (CommunicationResults_Object);
                }
              }
              CommunicationResults_Object.NumberOfObjetsChanged++;
            }
          }

          // Hent listen med nuværende CityId igen fra databasen. Der er sikkert slettet et eller flere
          // elementer i denne. Dette kan sikert gøres uden at skulle kalde databasen igen !!! => se på dette senere.
          CityLangualeListFromRepo = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetAllLanguagesFromCityId(CityIdSave);
        }
       
        foreach (var CityLanguageCombination in CityLangualeListFromRepo)
        {
          CurrentLanguageIds.Add(CityLanguageCombination.LanguageId);
        }

        for (ListCounter = 0; ListCounter < CityLanguageForSaveAndUpdateDto_List.Count; ListCounter++)
        {
          if (!CurrentLanguageIds.Contains(CityLanguageForSaveAndUpdateDto_List[ListCounter].LanguageId))
          {
            CityLanguage CityLanguage_Object = new CityLanguage();
            TypeAdapter.Adapt(CityLanguageForSaveAndUpdateDto_List[ListCounter], CityLanguage_Object);
            await _repositoryWrapper.CityLanguageRepositoryWrapper.Create(CityLanguage_Object);

            NumberOfObjectsChanged = await _repositoryWrapper.CityLanguageRepositoryWrapper.Save();
            if (1 != NumberOfObjectsChanged)
            {
              CommunicationResults_Object.ResultString = $"CityLanguage Object with CityId : {CityLanguage_Object.CityId} and LanguageId : {CityLanguage_Object.LanguageId} not saved for {UserName} in action UpdateCityLanguagesList";
              CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotModified;
              return (CommunicationResults_Object);
            }
            CommunicationResults_Object.NumberOfObjetsChanged++;
          }
        }

        if (!UseExtendedDatabaseDebugging) 
        {
          NumberOfObjectsActuallySaved = await _repositoryWrapper.CityLanguageRepositoryWrapper.Save();

          if (NumberOfObjectsActuallySaved != CommunicationResults_Object.NumberOfObjetsChanged)
          {
            CommunicationResults_Object.ResultString = $"Noget gik galt ved opdatering/slet af et eller flere objekter for {UserName} in action UpdateCityLanguagesList";
            CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.BadRequest;
            return (CommunicationResults_Object);
          }
        }

        CommunicationResults_Object.HasErrorOccured = false;
        CommunicationResults_Object.ResultString = $"Languagelist for CityId : {CityIdSave} er nu opdateret for for {UserName} in action UpdateCityLanguagesList. Number of objects changed : {CommunicationResults_Object.NumberOfObjetsChanged}";
        CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.Created;
        return (CommunicationResults_Object);
      }
      else
      {
        CommunicationResults_Object.HasErrorOccured = false;
        CommunicationResults_Object.ResultString = $"Ingenting i liste. Så intet er opdateret !!! for {UserName}";
        CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotImplemented;
        return (CommunicationResults_Object);
      }
    }
  }
}
