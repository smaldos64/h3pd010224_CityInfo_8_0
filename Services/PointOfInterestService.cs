using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Mapster;
using ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PointOfInterestService : IPointOfInterestService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public PointOfInterestService(IRepositoryWrapper repositoryWrapper)
        {
            this._repositoryWrapper = repositoryWrapper;
        }

        public async Task<ICommunicationResults> UpdatePointOfInterestListForCity(int CityId,
                                                                    List<PointOfInterestForUpdateDto> PointOfInterestForUpdateDto_List,
                                                                    bool DeleteOldElementsInListNotSpecifiedInCurrentList = true,
                                                                    string UserName = "No Name",
                                                                    bool UseExtendedDatabaseDebugging = false)
        {
            int NumberOfObjectsChanged = 0;
            int NumberOfObjectsActuallySaved = 0;
            List<int> AddedList = new List<int>();
            ICommunicationResults CommunicationResults_Object = new CommunicationResults(true);

            City CityFromRepo = await _repositoryWrapper.CityRepositoryWrapper.FindOne(CityId);

            if (null == CityFromRepo)
            {
                CommunicationResults_Object.ResultString = $"City With ID : {CityId} not found in Cities for {UserName} in action UpdatePointOfInterestForCity";
                CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotFound;
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

                        if (UseExtendedDatabaseDebugging)
                        {
                            //NumberOfObjectsChanged = await _repositoryWrapper.CityRepositoryWrapper.Save();
                            NumberOfObjectsChanged = await _repositoryWrapper.Save();

                            if (1 != NumberOfObjectsChanged)
                            {
                                CommunicationResults_Object.ResultString = $"PointOfInterest Object with Name : {PointOfInterest_Object.PointOfInterestName} not created for {UserName} in action UpdateCityWithAllRelations";
                                CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotModified;
                                return (CommunicationResults_Object);
                            }
                        }
                        CommunicationResults_Object.NumberOfObjetsChanged++;

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

                        if (UseExtendedDatabaseDebugging)
                        {
                            //NumberOfObjectsChanged = await _repositoryWrapper.CityRepositoryWrapper.Save();
                            NumberOfObjectsChanged = await _repositoryWrapper.Save();

                            if (1 != NumberOfObjectsChanged)
                            {
                                CommunicationResults_Object.ResultString = $"PointOfInterest Object with PointOfInterestId : {PointOfInterestFromRepo.PointOfInterestId} not updated for {UserName} in action UpdateCityWithAllRelations";
                                CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotModified;
                                return (CommunicationResults_Object);
                            }
                        }
                        CommunicationResults_Object.NumberOfObjetsChanged++;
                    }
                }

                if (true == DeleteOldElementsInListNotSpecifiedInCurrentList)
                {
                    var PointOfInterestList = await _repositoryWrapper.PointOfInterestRepositoryWrapper.GetAllPointOfInterestWithCityID(CityId, false);

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

                                if (UseExtendedDatabaseDebugging)
                                {
                                    //NumberOfObjectsChanged = await _repositoryWrapper.PointOfInterestRepositoryWrapper.Save();
                                    NumberOfObjectsChanged = await _repositoryWrapper.Save();

                                    if (1 != NumberOfObjectsChanged)
                                    {
                                        CommunicationResults_Object.ResultString = $"PointOfInterest Object with PointOfInterestId : {PointOfInterest_object.PointOfInterestId} not deleted for {UserName} in action UpdateCityWithAllRelations";
                                        CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotModified;
                                        return (CommunicationResults_Object);
                                    }
                                }
                                CommunicationResults_Object.NumberOfObjetsChanged++;
                            }
                        }
                    }

                    if (!UseExtendedDatabaseDebugging)
                    {
                        //NumberOfObjectsActuallySaved = await _repositoryWrapper.PointOfInterestRepositoryWrapper.Save();
                        NumberOfObjectsActuallySaved = await _repositoryWrapper.Save();

                        if (NumberOfObjectsActuallySaved != CommunicationResults_Object.NumberOfObjetsChanged)
                        {
                            CommunicationResults_Object.ResultString = $"Noget gik galt ved opdatering/slet af et eller flere objekter for {UserName} in action UpdatePointOfInterestForCity";
                            CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.BadRequest;
                            return (CommunicationResults_Object);
                        }
                    }

                    CommunicationResults_Object.HasErrorOccured = false;
                    CommunicationResults_Object.ResultString = $"PointOfInterstlist for CityId : {CityId} er nu opdateret for for {UserName} in action UpdatePointOfInterestForCity og tidligere PointOfInterests er slettet. Number of objects changed : {CommunicationResults_Object.NumberOfObjetsChanged}";
                    CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.Created;
                    return (CommunicationResults_Object);
                }
                else
                {
                    if (!UseExtendedDatabaseDebugging)
                    {
                        //NumberOfObjectsActuallySaved = await _repositoryWrapper.PointOfInterestRepositoryWrapper.Save();
                        NumberOfObjectsActuallySaved = await _repositoryWrapper.Save();

                        if (NumberOfObjectsActuallySaved != CommunicationResults_Object.NumberOfObjetsChanged)
                        {
                            CommunicationResults_Object.ResultString = $"Noget gik galt ved opdatering/slet af et eller flere objekter for {UserName} in action UpdatePointOfInterestForCity";
                            CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.BadRequest;
                            return (CommunicationResults_Object);
                        }
                    }

                    CommunicationResults_Object.HasErrorOccured = false;
                    CommunicationResults_Object.ResultString = $"PointOfInterstlist for CityId : {CityId} er nu opdateret for for {UserName} in action UpdatePointOfInterestForCity uden at slette tidligere PointOfInterests. Number of objects changed : {CommunicationResults_Object.NumberOfObjetsChanged}";
                    CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.Created;
                    return (CommunicationResults_Object);
                }
            }
            else
            {
                CommunicationResults_Object.HasErrorOccured = false;
                CommunicationResults_Object.ResultString = $"No PointOfInterest Object specified in list for {UserName} in action UpdatePointOfInterestForCity";
                CommunicationResults_Object.HttpStatusCodeResult = (int)HttpStatusCode.NotImplemented;
                return (CommunicationResults_Object);
            }
        }
    }
}
