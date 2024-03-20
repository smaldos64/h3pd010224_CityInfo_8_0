using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_UnitTests.Setup
{
    public class TestingWebAppFactory<T> : WebApplicationFactory<Program>
    {
        //public static IServiceProvider TestServiceProvider;
        //public static WebApplicationBuilder? WepApplicationBuilderToDistribute { get; set; }
        //public static IWebHostBuilder? WepApplicationBuilderToDistribute { get; set; }
        public static DatabaseContext _databaseContext { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //WebApplicationBuilder builder = WebApplication.CreateBuilder();
            
            //string args[];
            //IServiceCollection MyServiceCollection = new ServiceCollection();
            //WebHostBuilder MyBuilder = builder.ConfigureServices(MyServiceCollection);




            builder.ConfigureServices(services =>
            {
                var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

                // LTPE => Slet database context fra CityInfo_8_0_Server
                //if (dbContext != null)
                //{
                //    //services.Remove(dbContext);
                //    services.RemoveAll(typeof(DbContextOptions<DatabaseContext>));
                //} LTPE indsæt igen

                //var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
                var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase()
                                                             .AddEntityFrameworkProxies() 
                                                             .BuildServiceProvider();

                //services.AddDbContext<DatabaseContext>(options =>
                //{
                //    options.UseInMemoryDatabase("InMemoryDatabaseTest");
                //    options.UseInternalServiceProvider(serviceProvider);
                //}); LTPE indsæt igen

                //antiforgery
                //services.AddAntiforgery(t =>
                //{
                //    t.Cookie.Name = AntiForgeryTokenExtractor.Cookie;
                //    t.FormFieldName = AntiForgeryTokenExtractor.Field;
                //});

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    using (var appContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                    {
                        try
                        {
                            //appContext.Database.EnsureCreated();
                            
                            //SetupDatabaseData.SeedDatabaseData(appContext);
                            //SeedData(appContext);
                            _databaseContext = appContext;
                        }
                        catch (Exception ex)
                        {
                            //Log errors
                            throw;
                        }
                    }
                }
            });

            //WepApplicationBuilderToDistribute = builder;
            //DatabaseContext dbContext = builder
        }

        private void SeedData(DatabaseContext context1)
        {
            int NumberOfDatabaseObjectsChanged = 0;

            using (var context = context1)
            {
                List<Language> LanguageObjectList = new List<Language>()
                {
                    new Language
                    {
                        LanguageName = "dansk"
                    },
                    new Language
                    {
                        LanguageName = "engelsk"
                    },
                    new Language
                    {
                        LanguageName = "tysk"
                    }
                };
                context.AddRangeAsync(LanguageObjectList);
                NumberOfDatabaseObjectsChanged = context.SaveChanges();

                List<Country> CountryObjectList = new List<Country>()
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
                context.AddRangeAsync(CountryObjectList);
                NumberOfDatabaseObjectsChanged = context.SaveChanges();

                List<City> CityObjectList = new List<City>()
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
                context.AddRangeAsync(CityObjectList);
                NumberOfDatabaseObjectsChanged = context.SaveChanges();

                List<PointOfInterest> PointOfInterestObjectList = new List<PointOfInterest>()
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
                context.AddRangeAsync(PointOfInterestObjectList);
                NumberOfDatabaseObjectsChanged = context.SaveChanges();

                List<CityLanguage> CityLanguageObjectList = new List<CityLanguage>()
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
                context.AddRangeAsync(CityLanguageObjectList);
                NumberOfDatabaseObjectsChanged = context.SaveChanges();

                var Cities = context.Core_8_0_Cities.ToList();
            }
        }

        private void Seed(DatabaseContext context)
        {
            int NumberOfDatabaseObjectsChanged = 0;

            List<Language> LanguageObjectList = new List<Language>()
            {
                new Language
                {
                    LanguageName = "dansk"
                },
                new Language
                {
                    LanguageName = "engelsk"
                },
                new Language
                {
                    LanguageName = "tysk"
                }
            };
            context.AddRangeAsync(LanguageObjectList);
            NumberOfDatabaseObjectsChanged = context.SaveChanges();

            List<Country> CountryObjectList = new List<Country>()
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
            context.AddRangeAsync(CountryObjectList);
            NumberOfDatabaseObjectsChanged = context.SaveChanges();

            List<City> CityObjectList = new List<City>()
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
            context.AddRangeAsync(CityObjectList);
            NumberOfDatabaseObjectsChanged = context.SaveChanges();

            List<PointOfInterest> PointOfInterestObjectList =   new List<PointOfInterest>()
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
            context.AddRangeAsync(PointOfInterestObjectList);
            NumberOfDatabaseObjectsChanged = context.SaveChanges();

            List<CityLanguage> CityLanguageObjectList = new List<CityLanguage>()
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
            context.AddRangeAsync(CityLanguageObjectList);
            NumberOfDatabaseObjectsChanged = context.SaveChanges();

        }
    }
}
