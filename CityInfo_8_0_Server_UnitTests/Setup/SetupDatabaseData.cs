using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CityInfo_8_0_Server_UnitTests.ViewModels;

namespace CityInfo_8_0_Server_UnitTests.Setup
{
    public static class SetupDatabaseData
    {
        public static List<Language> LanguageObjectList = new List<Language>();
        public static List<Country> CountryObjectList = new List<Country>();
        public static List<City> CityObjectList = new List<City>();
        public static List<PointOfInterest> PointOfInterestObjectList = new List<PointOfInterest>();
        public static List<CityLanguage> CityLanguageObjectList = new List<CityLanguage>();

        public static async Task SeedDatabaseData(DatabaseContext context)
        {
            int NumberOfDatabaseObjectsChanged = 0;

            try
            {
                LanguageObjectList = new List<Language>()
                {
                    new Language
                    {
                        LanguageName = "Dansk"
                    },
                    new Language
                    {
                        LanguageName = "Engelsk"
                    },
                    new Language
                    {
                        LanguageName = "Tysk"
                    }
                };
                for (int Counter = 0; Counter < LanguageObjectList.Count; Counter++)
                {
                    await context.AddAsync(LanguageObjectList[Counter]);
                    context.ChangeTracker.Clear();
                    //Thread.Sleep(100);
                }
                //await context.AddRangeAsync(LanguageObjectList);
                NumberOfDatabaseObjectsChanged = await context.SaveChangesAsync();
                //Thread.Sleep(100);

                CountryObjectList = new List<Country>()
                {
                    new Country
                    {
                        CountryName = "Danmark"
                    },
                    new Country
                    {
                        CountryName = "England"
                    },
                    new Country
                    {
                        CountryName = "Tyskland"
                    },
                };
                for (int Counter = 0; Counter < CountryObjectList.Count; Counter++)
                {
                    await context.AddAsync(CountryObjectList[Counter]);
                    context.ChangeTracker.Clear();
                    //Thread.Sleep(100);
                }
                //await context.AddRangeAsync(CountryObjectList);
                NumberOfDatabaseObjectsChanged = await context.SaveChangesAsync();
                //Thread.Sleep(100);
                context.ChangeTracker.Clear();

                CityObjectList = new List<City>()
                {
                    new City
                    {
                        CityName = "Gudumholm",
                        CityDescription = "Østhimmerlands perle !!!",
                        CountryID = CountryObjectList[0].CountryID
                    },
                    new City
                    {
                        CityName = "London",
                        CityDescription = "Englands hovedstad",
                        CountryID = CountryObjectList[1].CountryID
                    },
                    new City
                    {
                        CityName = "Hamburg",
                        CityDescription = "Byen ved Elben",
                        CountryID = CountryObjectList[2].CountryID
                    }
                };
                for (int Counter = 0; Counter < CityObjectList.Count; Counter++)
                {
                    await context.AddAsync(CityObjectList[Counter]);
                    context.ChangeTracker.Clear();
                    //Thread.Sleep(100);
                }
                //await context.AddRangeAsync(CityObjectList);
                NumberOfDatabaseObjectsChanged = await context.SaveChangesAsync();
                //Thread.Sleep(100);
                context.ChangeTracker.Clear();

                PointOfInterestObjectList = new List<PointOfInterest>()
                {
                    new PointOfInterest
                    {
                        PointOfInterestName = "Gudumholm Stadion",
                        PointOfInterestDescription = "Her har Lars P spillet mange kampe",
                        CityId = CityObjectList[0].CityId
                    },
                    new PointOfInterest
                    {
                        PointOfInterestName = "Gudumholm Brugs",
                        PointOfInterestDescription = "Her regerer Jesper Baron Berthelsen",
                        CityId = CityObjectList[0].CityId
                    },
                    new PointOfInterest
                    {
                        PointOfInterestName = "Wembley",
                        PointOfInterestDescription = "Berømt fodboldstadion",
                        CityId = CityObjectList[1].CityId
                    },
                    new PointOfInterest
                    {
                        PointOfInterestName = "Elben tunnellen",
                        PointOfInterestDescription = "Letter trafikken gennem Hamburg",
                        CityId = CityObjectList[2].CityId
                    }
                };
                for (int Counter = 0; Counter < PointOfInterestObjectList.Count; Counter++)
                {
                    await context.AddAsync(PointOfInterestObjectList[Counter]);
                    context.ChangeTracker.Clear();
                    //Thread.Sleep(100);
                }
                //await context.AddRangeAsync(PointOfInterestObjectList);
                NumberOfDatabaseObjectsChanged = await context.SaveChangesAsync();
                //Thread.Sleep(100);
                context.ChangeTracker.Clear();

                CityLanguageObjectList = new List<CityLanguage>()
                {
                    new CityLanguage
                    {
                        CityId = CityObjectList[0].CityId,
                        LanguageId = LanguageObjectList[0].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = CityObjectList[0].CityId,
                        LanguageId = LanguageObjectList[1].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = CityObjectList[0].CityId,
                        LanguageId = LanguageObjectList[2].LanguageId
                    },

                    new CityLanguage
                    {
                        CityId = CityObjectList[1].CityId,
                        LanguageId = LanguageObjectList[1].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = CityObjectList[1].CityId,
                        LanguageId = LanguageObjectList[2].LanguageId
                    },

                    new CityLanguage
                    {
                        CityId = CityObjectList[2].CityId,
                        LanguageId = LanguageObjectList[1].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = CityObjectList[2].CityId,
                        LanguageId = LanguageObjectList[2].LanguageId
                    },
                };
                for (int Counter = 0; Counter < CityObjectList.Count; Counter++)
                {
                    await context.AddAsync(CityLanguageObjectList[Counter]);
                    context.ChangeTracker.Clear();
                    //Thread.Sleep(100);
                }
                //await context.AddRangeAsync(CityLanguageObjectList);
                NumberOfDatabaseObjectsChanged = await context.SaveChangesAsync();
                //Thread.Sleep(100);
                context.ChangeTracker.Clear();

                //var Cities = context.Core_8_0_Cities.ToList();
                //CityObjectList = await context.Core_8_0_Cities.ToListAsync();
            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
            }
        }

        public static async Task SeedDatabaseDataWithObject(DatabaseContext context, 
                                                            DatabaseViewModel databaseViewModel)
        {
            //List<Language> LanguageObjectList = new List<Language>();
            //List<Country> CountryObjectList = new List<Country>();
            //List<City> CityObjectList = new List<City>();
            //List<PointOfInterest> PointOfInterestObjectList = new List<PointOfInterest>();
            //List<CityLanguage> CityLanguageObjectList = new List<CityLanguage>();
            int NumberOfDatabaseObjectsChanged = 0;

            try
            {
                databaseViewModel.LanguageList = new List<Language>()
                {
                    new Language
                    {
                        LanguageName = "Dansk"
                    },
                    new Language
                    {
                        LanguageName = "Engelsk"
                    },
                    new Language
                    {
                        LanguageName = "Tysk"
                    }
                };
                await context.AddRangeAsync(databaseViewModel.LanguageList);
                NumberOfDatabaseObjectsChanged = await context.SaveChangesAsync();

                databaseViewModel.CountryList = new List<Country>()
                {
                    new Country
                    {
                        CountryName = "Danmark"
                    },
                    new Country
                    {
                        CountryName = "England"
                    },
                    new Country
                    {
                        CountryName = "Tyskland"
                    },
                };
                await context.AddRangeAsync(databaseViewModel.CountryList);
                NumberOfDatabaseObjectsChanged = await context.SaveChangesAsync();

                databaseViewModel.CityList = new List<City>()
                {
                    new City
                    {
                        CityName = "Gudumholm",
                        CityDescription = "Østhimmerlands perle !!!",
                        CountryID = databaseViewModel.CountryList[0].CountryID
                    },
                    new City
                    {
                        CityName = "London",
                        CityDescription = "Englands hovedstad",
                        CountryID = databaseViewModel.CountryList[1].CountryID
                    },
                    new City
                    {
                        CityName = "Hamburg",
                        CityDescription = "Byen ved Elben",
                        CountryID = databaseViewModel.CountryList[2].CountryID
                    }
                };
                await context.AddRangeAsync(databaseViewModel.CityList);
                NumberOfDatabaseObjectsChanged = await context.SaveChangesAsync();

                databaseViewModel.PointOfInterestList = new List<PointOfInterest>()
                {
                    new PointOfInterest
                    {
                        PointOfInterestName = "Gudumholm Stadion",
                        PointOfInterestDescription = "Her har Lars P spillet mange kampe",
                        CityId = databaseViewModel.CityList[0].CityId
                    },
                    new PointOfInterest
                    {
                        PointOfInterestName = "Gudumholm Brugs",
                        PointOfInterestDescription = "Her regerer Jesper Baron Berthelsen",
                        CityId = databaseViewModel.CityList[0].CityId
                    },
                    new PointOfInterest
                    {
                        PointOfInterestName = "Wembley",
                        PointOfInterestDescription = "Berømt fodboldstadion",
                        CityId = databaseViewModel.CityList[1].CityId
                    },
                    new PointOfInterest
                    {
                        PointOfInterestName = "Elben tunnellen",
                        PointOfInterestDescription = "Letter trafikken gennem Hamburg",
                        CityId = databaseViewModel.CityList[2].CityId
                    }
                };
                await context.AddRangeAsync(databaseViewModel.PointOfInterestList);
                NumberOfDatabaseObjectsChanged = await context.SaveChangesAsync();

                databaseViewModel.CityLanguageList = new List<CityLanguage>()
                {
                    new CityLanguage
                    {
                        CityId = databaseViewModel.CityList[0].CityId,
                        LanguageId = databaseViewModel.LanguageList[0].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = databaseViewModel.CityList[0].CityId,
                        LanguageId = databaseViewModel.LanguageList[1].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = databaseViewModel.CityList[0].CityId,
                        LanguageId = databaseViewModel.LanguageList[2].LanguageId
                    },

                    new CityLanguage
                    {
                        CityId = databaseViewModel.CityList[1].CityId,
                        LanguageId = databaseViewModel.LanguageList[1].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = databaseViewModel.CityList[1].CityId,
                        LanguageId = databaseViewModel.LanguageList[2].LanguageId
                    },

                    new CityLanguage
                    {
                        CityId = databaseViewModel.CityList[2].CityId,
                        LanguageId = databaseViewModel.LanguageList[1].LanguageId
                    },
                    new CityLanguage
                    {
                        CityId = databaseViewModel.CityList[2].CityId,
                        LanguageId = databaseViewModel.LanguageList[2].LanguageId
                    },
                };
                await context.AddRangeAsync(databaseViewModel.CityLanguageList);
                NumberOfDatabaseObjectsChanged = await context.SaveChangesAsync();
                
                //var Cities = context.Core_8_0_Cities.ToList();
                //CityObjectList = await context.Core_8_0_Cities.ToListAsync();
            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
            }
        }
    }
}
