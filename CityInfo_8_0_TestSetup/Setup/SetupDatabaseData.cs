using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CityInfo_8_0_TestSetup.ViewModels;
using Entities.DataTransferObjects;
using Mapster;

namespace CityInfo_8_0_TestSetup.Setup
{
    public static class SetupDatabaseData
    {
        public static async Task SeedDatabaseDataWithObject(DatabaseContext context,
                                                            DatabaseViewModel databaseViewModel,
                                                            bool IncludeRelations = false)
        {
            int NumberOfDatabaseObjectsChanged = 0;

            try
            {
                if (null == context)
                {
                    databaseViewModel.LanguageList = new List<Language>()
                    {
                        new Language
                        {
                            LanguageId = 1,
                            LanguageName = "Dansk"
                        },
                        new Language
                        {
                            LanguageId = 2,
                            LanguageName = "Engelsk"
                        },
                        new Language
                        {
                            LanguageId = 3,
                            LanguageName = "Tysk"
                        }
                    };
                }
                else
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
                }

                //databaseViewModel.LanguageDtoList = databaseViewModel.LanguageList.Adapt<LanguageDto[]>().ToList();  
                    
                if (null == context)
                {
                    databaseViewModel.CountryList = new List<Country>()
                    {
                        new Country
                        {
                            CountryID = 1,
                            CountryName = "Danmark"
                        },
                        new Country
                        {
                            CountryID = 2,
                            CountryName = "England"
                        },
                        new Country
                        {
                            CountryID = 3,
                            CountryName = "Tyskland"
                        },
                    };
                }
                else
                {
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
                }

                //databaseViewModel.CountryDtoList = databaseViewModel.CountryList.Adapt<CountryDto[]>().ToList();

                if (null == context)
                {
                    databaseViewModel.CityList = new List<City>()
                    {
                        new City
                        {
                            CityId = 1,
                            CityName = "Gudumholm",
                            CityDescription = "Østhimmerlands perle !!!",
                            CountryID = databaseViewModel.CountryList[0].CountryID
                        },
                        new City
                        {
                            CityId = 2,
                            CityName = "London",
                            CityDescription = "Englands hovedstad",
                            CountryID = databaseViewModel.CountryList[1].CountryID
                        },
                        new City
                        {
                            CityId = 3,
                            CityName = "Hamburg",
                            CityDescription = "Byen ved Elben",
                            CountryID = databaseViewModel.CountryList[2].CountryID
                        }
                    };
                }
                else
                {
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
                }

                //databaseViewModel.CityDtoList = databaseViewModel.CityList.Adapt<CityDto[]>().ToList();

                if (null == context)
                {
                    databaseViewModel.PointOfInterestList = new List<PointOfInterest>()
                    {
                        new PointOfInterest
                        {
                            PointOfInterestId = 1,
                            PointOfInterestName = "Gudumholm Stadion",
                            PointOfInterestDescription = "Her har Lars P spillet mange kampe",
                            CityId = databaseViewModel.CityList[0].CityId
                        },
                        new PointOfInterest
                        {
                            PointOfInterestId = 2,
                            PointOfInterestName = "Gudumholm Brugs",
                            PointOfInterestDescription = "Her regerer Jesper Baron Berthelsen",
                            CityId = databaseViewModel.CityList[0].CityId
                        },
                        new PointOfInterest
                        {
                            PointOfInterestId = 3,
                            PointOfInterestName = "Wembley",
                            PointOfInterestDescription = "Berømt fodboldstadion",
                            CityId = databaseViewModel.CityList[1].CityId
                        },
                        new PointOfInterest
                        {
                            PointOfInterestId= 4,
                            PointOfInterestName = "Elben tunnellen",
                            PointOfInterestDescription = "Letter trafikken gennem Hamburg",
                            CityId = databaseViewModel.CityList[2].CityId
                        }
                    };
                }
                else
                {
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
                }

                //databaseViewModel.PointOfInterestsDtoList = databaseViewModel.PointOfInterestList.Adapt<PointOfInterestDto[]>().ToList();

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
                if (null != context)
                {
                    await context.AddRangeAsync(databaseViewModel.CityLanguageList);
                    NumberOfDatabaseObjectsChanged = await context.SaveChangesAsync();
                }
                else
                {
                    if (true == IncludeRelations)
                    {
                        databaseViewModel.CityList[0].CityLanguages.Add(new CityLanguage
                        {
                            CityId = databaseViewModel.CityList[0].CityId,
                            LanguageId = databaseViewModel.LanguageList[0].LanguageId
                        });

                        databaseViewModel.CityList[0].CityLanguages.Add(new CityLanguage
                        {
                            CityId = databaseViewModel.CityList[0].CityId,
                            LanguageId = databaseViewModel.LanguageList[1].LanguageId
                        });

                        databaseViewModel.CityList[0].CityLanguages.Add(new CityLanguage
                        {
                            CityId = databaseViewModel.CityList[0].CityId,
                            LanguageId = databaseViewModel.LanguageList[2].LanguageId
                        });

                        databaseViewModel.CityList[1].CityLanguages.Add(new CityLanguage
                        {
                            CityId = databaseViewModel.CityList[1].CityId,
                            LanguageId = databaseViewModel.LanguageList[1].LanguageId
                        });

                        databaseViewModel.CityList[1].CityLanguages.Add(new CityLanguage
                        {
                            CityId = databaseViewModel.CityList[1].CityId,
                            LanguageId = databaseViewModel.LanguageList[2].LanguageId
                        });

                        databaseViewModel.CityList[2].CityLanguages.Add(new CityLanguage
                        {
                            CityId = databaseViewModel.CityList[2].CityId,
                            LanguageId = databaseViewModel.LanguageList[1].LanguageId
                        });

                        databaseViewModel.CityList[2].CityLanguages.Add(new CityLanguage
                        {
                            CityId = databaseViewModel.CityList[2].CityId,
                            LanguageId = databaseViewModel.LanguageList[2].LanguageId
                        });
                    }
                }

                //databaseViewModel.CityLanguageDtoList = databaseViewModel.CityLanguageList.Adapt<CityLanguageDto[]>().ToList();
            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
            }
        }

        public static async Task<IEnumerable<City>> GetAllCities(DatabaseViewModel databaseViewModel, bool IncludeRelations = false)
        {
            return (databaseViewModel.CityList.ToList());
        }

        public static List<City> GetAllCitiesAsList(DatabaseViewModel databaseViewModel, bool IncludeRelations = false)
        {
            return (databaseViewModel.CityList.ToList());
        }
    }
}
