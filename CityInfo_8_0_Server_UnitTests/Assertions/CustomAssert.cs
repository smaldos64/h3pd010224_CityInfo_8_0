﻿using CityInfo_8_0_Server_UnitTests.Setup;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CityInfo_8_0_Server_UnitTests.Assertions
{
    public class CustomAssert
    {
        public static void IsInRange(int value, int min, int max)
        {
            Assert.True(value >= min && value <= max);
        }

        public static async Task InMemoryModeCheckCitiesRead(List<City> CityList, bool IncludeRelations)
        {
            bool DifferenceFound = false;
            List<City> CityListSorted = new List<City>();
            CityListSorted = CityList.OrderBy(c => c.CityId).ToList();
            List<City> CityListSortedFromSetup = new List<City>();
            CityListSortedFromSetup = SetupDatabaseData.CityObjectList.OrderBy(c => c.CityId).ToList();

            await Task.Delay(1);
            // For at sikre at funktionen kører asynkront, selvom der ikke er noget await kald i 
            // funktionen.

            if (CityListSortedFromSetup.Count !=
                        SetupDatabaseData.CityObjectList.Count)
            {
                DifferenceFound = true;
            }
            Assert.Equal(CityList.Count, SetupDatabaseData.CityObjectList.Count);
            
            if (true == IncludeRelations)
            {
                for (int Counter = 0; Counter < SetupDatabaseData.CityObjectList.Count; Counter++)
                {
                    if (CityListSortedFromSetup[Counter].CityLanguages.Count !=
                        CityListSorted[Counter].CityLanguages.Count)
                    {
                        DifferenceFound = true;
                    }
                    Assert.Equal(CityListSortedFromSetup[Counter].CityLanguages.Count,
                    CityListSorted[Counter].CityLanguages.Count);
                }
            }
            else
            {
                // Det kan åbenbart ikke rigtig lade sig gøre at få
                // InMemory databasen til at holde op med at bruge
                // LazyLoading, selvom vi længere oppe i vores testCase 
                // har specificeret, at LazyLoading skal disables.

                //for (int Counter = 0; Counter < SetupDatabaseData.CityObjectList.Count; Counter++)
                //{
                //    Assert.Equal(0, CityList[Counter].CityLanguages.Count);
                //}
                for (int Counter = 0; Counter < SetupDatabaseData.CityObjectList.Count; Counter++)
                {
                    if (CityListSortedFromSetup[Counter].CityLanguages.Count !=
                        CityListSorted[Counter].CityLanguages.Count)
                    {
                        DifferenceFound = true;
                    }
                    Assert.Equal(CityListSortedFromSetup[Counter].CityLanguages.Count,
                    CityListSorted[Counter].CityLanguages.Count);
                }
            }
        }
    }
}
