using CityInfo_8_0_TestSetup.Setup;
using CityInfo_8_0_TestSetup.ViewModels;
using Entities.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
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
                                                                       bool SqlDatabaseUsed = false,
                                                                       int NumberOfCitiesToCheckFor = 0)
        {
            List<City> CityListSorted = new List<City>();
            CityListSorted = CityList.OrderBy(c => c.CityId).ToList();

            await Task.Delay(1);
            // For at sikre at funktionen kører asynkront, selvom der ikke er noget await kald i 
            // funktionen.

            if (0 == NumberOfCitiesToCheckFor)
            {
                Assert.Equal(CityList.Count, databaseViewModel.CityList.Count);
            }

            if (true == IncludeRelations)
            {
                for (int Counter = 0; Counter < CityList.Count; Counter++)
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
                    for (int Counter = 0; Counter < CityList.Count; Counter++)
                    {
                        Assert.Equal(databaseViewModel.CityList[Counter].CityLanguages.Count,
                        CityListSorted[Counter].CityLanguages.Count);
                    }
                }
                else
                {
                    for (int Counter = 0; Counter < CityList.Count; Counter++)
                    {
                        Assert.Empty(CityListSorted[Counter].CityLanguages);
                    }
                }
            }
        }

        public static bool AreListOfObjectsEqualByFields<T>(List<T> List1, List<T> List2, bool CompareLists = false)
        {
            int Counter = 0;
            do
            {
                if (!AreObjectsEqualByFields<T>(List1[Counter], List2[Counter], CompareLists))
                {
                    return (false);
                }
                else
                {
                    Counter++;
                }
            } while (Counter < List1.Count);

            return (true);
        }

        public static bool AreObjectsEqualByFields<T>(T obj1, T obj2, bool compareLists = false, int NumberOfElementsToCheck = 0)
        {
            dynamic obj1Dynamic = obj1;
            dynamic obj2Dynamic = obj2;

            if (obj1Dynamic is null || obj2Dynamic is null)
            {
                return obj1Dynamic == obj2Dynamic; // Handle nulls
            }

            Type ObjectType = obj1.GetType();
            var fields = ObjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            //var type = typeof(T);
            //var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var value1 = field.GetValue(obj1);
                var value2 = field.GetValue(obj2);

                if (value1 == null || value2 == null)
                {
                    if (!ReferenceEquals(value1, value2))
                    {
                        return false; // Handle both null and one-null cases
                    }
                }
                else if (compareLists && value1 is IEnumerable && value2 is IEnumerable)
                {
                    if (!AreEqualCollections((IEnumerable)value1, (IEnumerable)value2, compareLists))
                    {
                        return false; // Recursively compare lists
                    }
                }
                else if (!value1.Equals(value2))
                {
                    return false; // Regular value comparison
                }
            }

            return true;
        }

        public static bool AreEqualCollections(IEnumerable collection1, IEnumerable collection2, bool compareLists)
        {
            dynamic collection1Count = collection1;
            dynamic collection2Count = collection2;

            if (collection1 is null || collection2 is null)
            {
                return collection1 == collection2; // Handle null collections
            }

            if (collection1Count.Count() != collection2Count.Count())
            {
                return false; // Different sizes
            }

            var enumerator1 = collection1.GetEnumerator();
            var enumerator2 = collection2.GetEnumerator();

            while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
                var item1 = enumerator1.Current;
                var item2 = enumerator2.Current;

                if (item1 is IEnumerable && item2 is IEnumerable && compareLists)
                {
                    if (!AreEqualCollections((IEnumerable)item1, (IEnumerable)item2, compareLists))
                    {
                        return false; // Recursively compare nested lists
                    }
                }
                else if (!item1.Equals(item2))
                {
                    return false; // Compare individual items
                }
            }

            return true;
        }
    }
}
