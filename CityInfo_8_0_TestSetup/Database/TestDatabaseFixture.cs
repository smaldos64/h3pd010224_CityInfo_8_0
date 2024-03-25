using CityInfo_8_0_TestSetup.Setup;
using CityInfo_8_0_TestSetup.ViewModels;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_TestSetup.Database
{
    public class TestDatabaseFixture
    {
        public DatabaseViewModel DatabaseViewModelObject;
        public const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=CityInfoDB_Core_8_0_Module_Test;User ID=ltpe2;Password=skolelogin";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        // Kode her bliver kun kaldt én gang for hver Tast klasse fil. 

        public TestDatabaseFixture()
        {
            Task.Run(async () =>
            {
                //lock (_lock)
                //{
                    if (!_databaseInitialized)
                    {
                        var context = CreateContext();
                        await context.Database.EnsureDeletedAsync();
                        await context.Database.EnsureCreatedAsync();
                            
                        DatabaseViewModelObject = new DatabaseViewModel();
                        await SetupDatabaseData.SeedDatabaseDataWithObject(context, DatabaseViewModelObject);
                       
                        _databaseInitialized = true;
                    }
                  //}
            }).GetAwaiter().GetResult();
        }

        public DatabaseContext CreateContext()
            => new SQLDatabaseUnitTestDatabaseContext(
                new DbContextOptionsBuilder<DatabaseContext>()
                    .UseSqlServer(ConnectionString)
                    .Options, null);
    }
}
