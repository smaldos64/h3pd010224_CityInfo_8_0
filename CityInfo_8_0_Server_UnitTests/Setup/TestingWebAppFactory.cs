using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

                // LTPE => Slet database context fra CityInfo_8_0_Server
                if (dbContext != null)
                    services.Remove(dbContext);

                var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDatabaseTest");
                    options.UseInternalServiceProvider(serviceProvider);
                });

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
                            appContext.Database.EnsureCreated();
                            Seed(appContext);
                        }
                        catch (Exception ex)
                        {
                            //Log errors
                            throw;
                        }
                    }
                }

            });
        }

        private void Seed(DatabaseContext context)
        {
            //Language LanguageObject = new Language()
            //{
            //    LanguageId = 1,
            //    LanguageName = "Danmark"
            //};

            List<Language> LanguageObjectList = new List<Language>()
            {
                new Language
                {
                    //LanguageId = 1,
                    LanguageName = "dansk"
                },
                new Language
                {
                    //LanguageId = 2,
                    LanguageName = "engelsk"
                },
                new Language
                {
                    //LanguageId = 3,
                    LanguageName = "tysk"
                }
            };
            context.AddRangeAsync(LanguageObjectList);
            context.SaveChanges();

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
            context.SaveChanges();

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
            context.SaveChanges();

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
            context.SaveChanges();

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
            context.SaveChanges();

            //context.AddRange(one, two, three);
            //context.SaveChanges();
        }
    }
}
