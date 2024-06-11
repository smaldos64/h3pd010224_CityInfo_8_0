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
        private readonly ICityLanguageService _cityLanguage;
        private readonly IPointOfInterestService _pointOfInterestService;

        public CityService(IRepositoryWrapper repositoryWrapper,
                           ICityLanguageService cityLanguage,
                           IPointOfInterestService pointOfInterestService)
        {
            this._repositoryWrapper = repositoryWrapper;
            this._cityLanguage = cityLanguage;
            this._pointOfInterestService = pointOfInterestService;
        }

        public async Task<IEnumerable<City>> GetAllCities(bool IncludeRelations = false)
        {
            return (await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(IncludeRelations));
        }

        public async Task<int> SaveCity(City City_Object)
        {
            int NumberOfObjectsChanged;

            try
            {
                await _repositoryWrapper.CityRepositoryWrapper.Create(City_Object);
                NumberOfObjectsChanged = await _repositoryWrapper.Save();

                return (NumberOfObjectsChanged);
            }
            catch (Exception Error)
            {
                return (0);
            }
        }

        public async Task<ICommunicationResults> SaveCityWithAllInfo(CityForSaveDto CityForSaveDto_Object,
                                                                     List<PointOfInterestForSaveDto> PointOfInterestsForSaveDto_List,
                                                                     List<CityLanguageForSaveAndUpdateDto> CityLanguageForSaveAndUpdateDto_list,
                                                                     string UserName = "No Name",
                                                                     bool UseExtendedDatabaseDebugging = false)
        {
            int NumberOfObjectsSaved = 0;
            int NumberOfObjectsActuallySaved = 0;
            List<PointOfInterestForUpdateDto> PointOfInterestForUpdateDto_List = new List<PointOfInterestForUpdateDto>();
            ICommunicationResults CommunicationResults_Object = new CommunicationResults(true);
            City City_Object;

            try
            {
                City_Object = CityForSaveDto_Object.Adapt<City>();

                await _repositoryWrapper.CityRepositoryWrapper.Create(City_Object);
                NumberOfObjectsSaved = await _repositoryWrapper.Save();
            }
            catch (Exception Error)
            {
                CommunicationResults_Object.ResultString = Error.Message;
                CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.InternalServerError;
                return (CommunicationResults_Object);
            }

            NumberOfObjectsActuallySaved = NumberOfObjectsSaved;

            CityForUpdateDto CityForUpdateDto_Object = City_Object.Adapt<CityForUpdateDto>();

            PointOfInterestForUpdateDto_List = PointOfInterestsForSaveDto_List.Adapt<PointOfInterestForUpdateDto[]>().ToList();
            for (int Counter = 0; Counter < PointOfInterestForUpdateDto_List.Count; Counter++)
            {
                PointOfInterestForUpdateDto_List[Counter].CityId = CityForUpdateDto_Object.CityId;
            }

            for (int Counter = 0; Counter < CityLanguageForSaveAndUpdateDto_list.Count; Counter++)
            {
                CityLanguageForSaveAndUpdateDto_list[Counter].CityId = CityForUpdateDto_Object.CityId;
            }

            CommunicationResults_Object = await UpdateCityWithAllRelations(CityForUpdateDto_Object,
                                                                           PointOfInterestForUpdateDto_List,
                                                                           CityLanguageForSaveAndUpdateDto_list,
                                                                           UserName,
                                                                           false,
                                                                           UseExtendedDatabaseDebugging,
                                                                           true);

            if (true == CommunicationResults_Object.HasErrorOccured)
            {
                return CommunicationResults_Object;
            }
            else
            {
                CommunicationResults_Object.NumberOfObjetsChanged += NumberOfObjectsActuallySaved;

                CommunicationResults_Object.ResultString = $"City and all relations has been saved for {UserName} in action SaveCityWithAllInfo. Number of objects changed : {CommunicationResults_Object.NumberOfObjetsChanged}";
                CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.OK;
                CommunicationResults_Object.HasErrorOccured = false;
                return (CommunicationResults_Object);
            }
        }

        public async Task<ICommunicationResults> UpdateCityWithAllRelations(CityForUpdateDto CityForUpdateDto_Object,
                                                    List<PointOfInterestForUpdateDto> PointOfInterestForUpdateDto_List,
                                                    List<CityLanguageForSaveAndUpdateDto> CityLanguageForSaveAndUpdateDto_List,
                                                    string UserName,
                                                    bool DeleteOldElementsInListsNotSpecifiedInCurrentLists = true,
                                                    bool UseExtendedDatabaseDebugging = false,
                                                    bool CalledFromSaveCityOperation = false)
        {
            int NumberOfObjectsChanged = 0;
            int NumberOfObjectsActuallySaved = 0;
            List<int> AddedList = new List<int>();
            ICommunicationResults CommunicationResults_Object = new CommunicationResults(true);

            if (!CalledFromSaveCityOperation)
            {
                City CityFromRepo = await _repositoryWrapper.CityRepositoryWrapper.FindOne(CityForUpdateDto_Object.CityId);

                if (null == CityFromRepo)
                {
                    CommunicationResults_Object.ResultString = $"City With ID : {CityForUpdateDto_Object.CityId} not found in Cities for {UserName} in action UpdateCityWithAllRelations";
                    CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotFound;
                    return (CommunicationResults_Object);
                }

                TypeAdapter.Adapt(CityForUpdateDto_Object, CityFromRepo);

                NumberOfObjectsChanged = await _repositoryWrapper.Save();
            }


            // Udelad check herunder. Vi kan være i den situation, hvor vi ikke ønsker at opdatere 
            // noget i City, men "kun" vil opdatere i en af de tilhørende lister.
            //if (1 != NumberOfObjectsChanged)
            //{
            //  CommunicationResults_Object.ResultString = $"City with Id : {CityForUpdateDto_Object.CityId} not updated for {UserName} in action UpdateCityWithAllRelations";
            //  CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotModified;
            //  return (CommunicationResults_Object);
            //}
            
            //CommunicationResults_Object.NumberOfObjetsChanged++;
            CommunicationResults_Object.NumberOfObjetsChanged += NumberOfObjectsChanged;

            if (null != PointOfInterestForUpdateDto_List)
            {
                NumberOfObjectsActuallySaved = CommunicationResults_Object.NumberOfObjetsChanged;
                CommunicationResults_Object = await this._pointOfInterestService.UpdatePointOfInterestListForCity(CityForUpdateDto_Object.CityId,
                                                                                             PointOfInterestForUpdateDto_List,
                                                                                             DeleteOldElementsInListsNotSpecifiedInCurrentLists,
                                                                                             UserName,
                                                                                             UseExtendedDatabaseDebugging);
                if (true == CommunicationResults_Object.HasErrorOccured)
                {
                    return CommunicationResults_Object;
                }
                CommunicationResults_Object.NumberOfObjetsChanged += NumberOfObjectsActuallySaved;
            }

            if (null != CityLanguageForSaveAndUpdateDto_List)
            {
                NumberOfObjectsActuallySaved = CommunicationResults_Object.NumberOfObjetsChanged;
                CommunicationResults_Object = await this._cityLanguage.UpdateCityLanguagesList(CityLanguageForSaveAndUpdateDto_List,
                                                                                               DeleteOldElementsInListsNotSpecifiedInCurrentLists,
                                                                                               UserName,
                                                                                               UseExtendedDatabaseDebugging);
                if (true == CommunicationResults_Object.HasErrorOccured)
                {
                    return CommunicationResults_Object;
                }
                CommunicationResults_Object.NumberOfObjetsChanged += NumberOfObjectsActuallySaved;
            }

            CommunicationResults_Object.ResultString = $"City and all relations has been updated/saved for {UserName} in action UpdateCityWithAllRelations. Number of objects changed : {CommunicationResults_Object.NumberOfObjetsChanged}";
            CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.OK;
            CommunicationResults_Object.HasErrorOccured = false;
            return (CommunicationResults_Object);
        }
    }
}
