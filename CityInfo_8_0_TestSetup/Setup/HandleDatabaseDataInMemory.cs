using CityInfo_8_0_TestSetup.ViewModels;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;

namespace CityInfo_8_0_TestSetup.Setup
{
    public class HandleDatabaseDataInMemory
    {
        public static void AddCityToDatabaseDataInMemory(DatabaseViewModel databaseViewModel,
                                                         City CityObject)
        {
            databaseViewModel.CityList.Add(CityObject);
        }

        public static void UpdateCityInDatabaseDataInMemory(DatabaseViewModel databaseViewModel,
                                                            City CityObject)
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
    }
}
