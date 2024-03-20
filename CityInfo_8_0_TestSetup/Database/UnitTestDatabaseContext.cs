﻿using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_TestSetup.Database
{
    public class UnitTestDatabaseContext : DatabaseContext
    {
        public UnitTestDatabaseContext(DbContextOptions<DatabaseContext> options,
                                       IConfiguration configuration) : base(options, configuration)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder
            //    .UseLazyLoadingProxies()
            //    .UseSqlServer(TestDatabaseFixture.ConnectionString);
        }
    }
}
