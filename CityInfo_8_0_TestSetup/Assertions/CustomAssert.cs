using CityInfo_8_0_TestSetup.Setup;
using CityInfo_8_0_TestSetup.ViewModels;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CityInfo_8_0_TestSetup.Assertions
{
    public class CustomAssert
    {
        public static void IsInRange(int value, int min, int max)
        {
            Assert.True(value >= min && value <= max);
        }
                
        public static async Task InMemoryModeCheckCitiesReadWithObject(List<City> CityList, 
                                                                       DatabaseViewModel databaseViewModel,
                                                                       bool IncludeRelations,
                                                                       bool SqlDatabaseUsed = false)
        {
            List<City> CityListSorted = new List<City>();
            CityListSorted = CityList.OrderBy(c => c.CityId).ToList();

            await Task.Delay(1);
            // For at sikre at funktionen kører asynkront, selvom der ikke er noget await kald i 
            // funktionen.

            Assert.Equal(CityList.Count, databaseViewModel.CityList.Count);

            if (true == IncludeRelations)
            {
                for (int Counter = 0; Counter < databaseViewModel.CityList.Count; Counter++)
                {
                    Assert.Equal(databaseViewModel.CityList[Counter].CityLanguages.Count,
                    CityListSorted[Counter].CityLanguages.Count);
                }
            }
            else
            {
                // Det kan åbenbart ikke rigtig lade sig gøre at få
                // InMemory databasen til at holde op med at bruge
                // LazyLoading, selvom vi længere oppe i vores testCase 
                // har specificeret, at LazyLoading skal disables.
                // Det er kun, når vi tester op imod en "rigtig" SQL Database,
                // at vi kan teste for om IncludeRelations = false bevirker,
                // at vi ikke får læst relaterede data med ud.
                if (false == SqlDatabaseUsed)
                {
                    for (int Counter = 0; Counter < databaseViewModel.CityList.Count; Counter++)
                    {
                        Assert.Equal(databaseViewModel.CityList[Counter].CityLanguages.Count,
                        CityListSorted[Counter].CityLanguages.Count);
                    }
                }
                else
                {
                    for (int Counter = 0; Counter < databaseViewModel.CityList.Count; Counter++)
                    {
                        Assert.Empty(CityListSorted[Counter].CityLanguages);
                    }
                }
            }
        }
    }
}
