using CityInfo_8_0_Server_IntegrationTests.Setup;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_IntegrationTests.Setup
{
    public class TestingWebAppFactory<T> : WebApplicationFactory<Program> where T : class
    {
        public static DatabaseContext _databaseContext { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
          builder.ConfigureServices(services =>
          {
            // Remove the app's DbContextOptions registration.
            ServiceDescriptor? dbContextOptionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<DatabaseContext>));
            if (dbContextOptionsDescriptor != null) services.Remove(dbContextOptionsDescriptor);

            // Create DbContextOptions using an in-memory database for testing.
            DbContextOptions dbContextOptions = new DbContextOptionsBuilder()
                // .UseSqlite("DataSource=:memory:")
                .UseInMemoryDatabase("InMemoryDbForTesting-" + Guid.NewGuid())
                .Options;

            // Remove the app's registrations.
            ServiceDescriptor? sqlDbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DatabaseContext));
            if (sqlDbContextDescriptor != null)
            {
              services.Remove(sqlDbContextDescriptor);
            }

            //ServiceDescriptor? dchUnitOfWorkDescriptor = services.SingleOrDefault(
            //    d => d.ServiceType ==
            //         typeof(DchUnitOfWork));
            //if (dchUnitOfWorkDescriptor != null) services.Remove(dchUnitOfWorkDescriptor);

            // Add new registrations
            //DatabaseContext sqlDbContext = new TestSqlDbContext(dbContextOptions);

            // LTPE => 
            //DatabaseContext sqlDbContext = new DatabaseContext(dbContextOptions);
            //services.AddSingleton(sqlDbContext);

            _databaseContext = new DatabaseContext(dbContextOptions);
            //services.AddSingleton(_databaseContext);
            // => LTPE

            //DchUnitOfWork dchUnitOfWork = new(sqlDbContext);
            //services.AddSingleton(dchUnitOfWork);
            //if (!dchUnitOfWork.IsServiceAvailable())
            //  throw new Exception("Database connection can not be made at this time, please try again another time");

            // Build the service provider.
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database contexts
            using IServiceScope scope = serviceProvider.CreateScope();
            IServiceProvider scopedServices = scope.ServiceProvider;
            DatabaseContext db = scopedServices.GetRequiredService<DatabaseContext>();

            // Ensure the database is created.
            // LTPE
            bool TestDb = false;
            TestDb = db.Database.EnsureCreated();
            TestDb = _databaseContext.Database.EnsureCreated();
            SetupDatabaseData.SeedDatabaseData(db);
            SetupDatabaseData.SeedDatabaseData(_databaseContext);
          });

          //builder.ConfigureTestServices(services =>
          //{
          //  services.AddScoped<DchPermissionService, TestDchPermissionService>();
          //});

          builder.UseEnvironment("Development");


      //builder.ConfigureServices(services =>
      //{
      //    var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

      //    // LTPE => Slet database context fra CityInfo_8_0_Server
      //    if (dbContext != null)
      //    {
      //        //services.Remove(dbContext);
      //        services.RemoveAll(typeof(DbContextOptions<DatabaseContext>));
      //    }

      //    var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase()
      //                                                 .AddEntityFrameworkProxies() 
      //                                                 .BuildServiceProvider();

      //    services.AddDbContext<DatabaseContext>(options =>
      //    {
      //        options.UseInMemoryDatabase("InMemoryDatabaseTest");
      //        options.UseInternalServiceProvider(serviceProvider);
      //    });

      //    services.AddSingleton(DatabaseContext);

      //  //antiforgery
      //  services.AddAntiforgery(t =>
      //  {
      //    t.Cookie.Name = AntiForgeryTokenExtractor.Cookie;
      //    t.FormFieldName = AntiForgeryTokenExtractor.Field;
      //  });

      //  var sp = services.BuildServiceProvider();

      //    using (var scope = sp.CreateScope())
      //    {
      //        using (DatabaseContext appContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
      //        {
      //            try
      //            {
      //                appContext.Database.EnsureCreated();
      //                //appContext.Database.EnsureDeleted();
      //                _databaseContext = appContext;
      //                //SetupDatabaseData.SeedDatabaseData(_databaseContext);
      //                //SeedData(appContext);
      //                //_databaseContext = appContext;
      //            }
      //            catch (Exception ex)
      //            {
      //                //Log errors
      //                throw;
      //            }
      //        }
      //    }
      //});
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
    }
}
