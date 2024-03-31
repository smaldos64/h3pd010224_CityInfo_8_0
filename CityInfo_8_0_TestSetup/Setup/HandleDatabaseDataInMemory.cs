using CityInfo_8_0_TestSetup.ViewModels;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Entities;

namespace CityInfo_8_0_TestSetup.Setup
{
    public class HandleDatabaseDataInMemory
    {
        public static void AddCityToDatabaseDataInMemory(DatabaseViewModel databaseViewModel,
                                                         City CityObject)
        {
            databaseViewModel.CityList.Add(CityObject);
        }

        public static async Task<IEnumerable<City>> FindAllCities(DatabaseViewModel databaseViewModel,
                                                           bool IncludeRelations)
        {
            await Task.Delay(1);
            return databaseViewModel.CityList;
        }

        //public static async Task<IEnumerable<T>> FindAllItems(DatabaseViewModel databaseViewModel,
        //                                                      bool IncludeRelations) where T : class
        //{
        //    await Task.Delay(1);
        //    var list = databaseViewModel.GetType().GetProperties()
        //              .FirstOrDefault(prop => prop.PropertyType == typeof(List<T>))?.GetValue(baseClass) as List<T>;
        //    return databaseViewModel.CityList;
        //}

        public static IEnumerable<T> GetListFromBaseClass<T>(DatabaseViewModel databaseViewModel, bool IncludeRelations) where T : class
        {
            // Get the appropriate list based on the generic type T
            var list = databaseViewModel.GetType().GetProperties()
                       .FirstOrDefault(prop => prop.PropertyType == typeof(List<T>))?.GetValue(databaseViewModel) as List<T>;

            return list; // Return the matching list or null if not found
        }

        public static void UpdateCityInDatabaseDataInMemory(DatabaseViewModel databaseViewModel,
                                                            City CityObject,
                                                            bool AddToExistingLists = false)
        {
            int Index = databaseViewModel.CityList.FindIndex(c => c.CityId == CityObject.CityId);

            if (Index != -1)
            {
                //databaseViewModel.CityList[Index] = CityObject;
                //databaseViewModel.CityList[Index] = CityObject.Adapt<City>();

                // Er nødt til at gøre det manuelt her, da vi ikke ønsker at berøre
                // CityLangueges - og PointOfInterest listerne !!!
                databaseViewModel.CityList[Index].CityName = CityObject.CityName;
                databaseViewModel.CityList[Index].CityDescription = CityObject.CityDescription;
                databaseViewModel.CityList[Index].CountryID = CityObject.CountryID;

                if (!AddToExistingLists)
                {
                    databaseViewModel.CityList[Index].PointsOfInterest = new List<PointOfInterest>();
                    databaseViewModel.CityList[Index].PointsOfInterest = CityObject.PointsOfInterest;

                    databaseViewModel.CityList[Index].CityLanguages = new List<CityLanguage>();
                    databaseViewModel.CityList[Index].CityLanguages = CityObject.CityLanguages;
                }
                else
                {
                    List<PointOfInterest> PointOfInterestsList = new List<PointOfInterest>();
                    PointOfInterestsList = CityObject.PointsOfInterest.ToList();
                    for (int Counter = 0; Counter < CityObject.PointsOfInterest.Count; Counter++)
                    {
                        databaseViewModel.CityList[Index].PointsOfInterest.Add(PointOfInterestsList[Counter]);
                    }

                    List<CityLanguage> CityLanguagesList = new List<CityLanguage>();
                    CityLanguagesList = CityObject.CityLanguages.ToList();
                    for (int Counter = 0; Counter < CityObject.CityLanguages.Count; Counter++)
                    {
                        databaseViewModel.CityList[Index].CityLanguages.Add(CityLanguagesList[Counter]);
                    }
                }

            }
        }

        public static void DeleteCityInDatabaseDataInMemory(DatabaseViewModel databaseViewModel,
                                                            int CityId)
        {
            int Index = databaseViewModel.CityList.FindIndex(c => c.CityId == CityId);

            if (Index != -1)
            {
                databaseViewModel.CityList.RemoveAt(Index);
            }
        }

        public static int FindCityWithSpecifiedCityId(DatabaseViewModel databaseViewModel,
                                                      int CityId)
        {
            int Counter = 0;

            do
            {
                if (CityId == databaseViewModel.CityList[Counter].CityId)
                {
                    return Counter;
                }
            } while (Counter < databaseViewModel.CityList.Count);

            return -1;
        }
    }
}
