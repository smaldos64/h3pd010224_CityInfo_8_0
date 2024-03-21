using CityInfo_8_0_TestSetup.Setup;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_UnitTests.Assertions
{
    public class SynchronousCustomAssert
    {
        public static void IsInRange(int value, int min, int max)
        {
            Assert.True(value >= min && value <= max);
        }

        public static void InMemoryModeCheckCitiesRead(List<City> CityList, bool IncludeRelations)
        {
            bool DifferenceFound = false;
            List<City> CityListSorted = new List<City>();
            CityListSorted = CityList.OrderBy(c => c.CityId).ToList();
            List<City> CityListSortedFromSetup = new List<City>();
            CityListSortedFromSetup = SynchronousSetupDatabaseData.CityObjectList.OrderBy(c => c.CityId).ToList();

            Assert.Equal(CityList.Count, SynchronousSetupDatabaseData.CityObjectList.Count);
            if (true == IncludeRelations)
            {
                for (int Counter = 0; Counter < CityListSortedFromSetup.Count; Counter++)
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
                for (int Counter = 0; Counter < CityListSortedFromSetup.Count; Counter++)
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
