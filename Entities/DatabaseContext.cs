using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
  public class DatabaseContext : DbContext
  {
    // LTPE Test Purpose
    private readonly string _connectionString;
    //private readonly Action<DatabaseContext, ModelBuilder> _modelCustomizer;

    private static string _sQLConnectionString = String.Empty;

    public static string SQLConnectionString
    {
      get
      {
        return _sQLConnectionString;
      }
      set
      {
        if (!String.IsNullOrEmpty(value))
        {
          _sQLConnectionString = value;
        }
      }
    }

    private readonly IConfiguration _configuration;
    public virtual DbSet<Country> Core_8_0_Countries { get; set; }
    public virtual DbSet<City> Core_8_0_Cities { get; set; }
    public virtual DbSet<PointOfInterest> Core_8_0_PointsOfInterest { get; set; }

    public virtual DbSet<Language> Core_8_0_Languages { get; set; }

    public virtual DbSet<CityLanguage> Core_8_0_CityLanguages { get; set; }

        
    // Constructor herunder bliver kaldt under normal kørsel.
    public DatabaseContext(DbContextOptions<DatabaseContext> options,
                            IConfiguration configuration) : base(options)
    {
        this._configuration = configuration;
    }
       
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<CityLanguage>()
          .HasKey(cl => new
          {
            cl.CityId,
            cl.LanguageId
          });

      base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      string connectionString;

#if ENABLED_FOR_LAZY_LOADING_USAGE
            //if (!Environment.GetEnvironmentVariable("IsTest").Equals("true", StringComparison.OrdinalIgnoreCase))
            //{
            //    connectionString = this._configuration.GetConnectionString("cityInfoDBConnectionString");

            //    optionsBuilder
            //        .UseLazyLoadingProxies()
            //        .UseSqlServer(connectionString);
            //}
            //else
            //{
            //    //optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString()); // Unique in-memory database name
            //}

            if (!String.IsNullOrEmpty(_sQLConnectionString))
            {
                connectionString = _sQLConnectionString;
            }
            else
            {
                connectionString = this._configuration.GetConnectionString("cityInfoDBConnectionString");
                optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(connectionString);
            }
            //optionsBuilder
            //    .UseLazyLoadingProxies()
            //    .UseSqlServer(connectionString);
#endif
        }

  }
}
