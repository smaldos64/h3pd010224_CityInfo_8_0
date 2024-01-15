using Microsoft.EntityFrameworkCore;
using Contracts;
using Repository;

using Entities;
using LoggerService;
using ServicesContracts;
using Services;

namespace CityInfo_8_0_Server.Extensions
{
  public static class ServiceExtensions
  {
    public static void ConfigureCors(this IServiceCollection services)
    {
      services.AddCors(options =>
      {
        options.AddPolicy("CorsPolicy",
            builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
      });
    }

    public static void ConfigureIISIntegration(this IServiceCollection services)
    {
      services.Configure<IISOptions>(options =>
      {

      });
    }

    public static void ConfigureLoggerService(this IServiceCollection services)
    {
      services.AddSingleton<ILoggerManager, LoggerManager>();
    }

    //public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
    //{
    //  var connectionString = config["mysqlconnection:connectionString"];

    //  services.AddDbContext<RepositoryContext>(o => o.UseMySql(connectionString,
    //      MySqlServerVersion.LatestSupportedServerVersion));
    //}

    public static void ConfigureMsSqlContext(this IServiceCollection services, IConfiguration config)
    {
      var connectionString = config["ConnectionStrings:cityInfoDBConnectionString"];

      //services.AddDbContext<DatabaseContext>(o => o.UseMySql(connectionString,
      //    MySqlServerVersion.LatestSupportedServerVersion, b => b.MigrationsAssembly("Entities")));

      services.AddDbContext<DatabaseContext>(o => o.UseSqlServer(connectionString, x => x.MigrationsAssembly("Entities")));
      
      //services.AddDbContext<DatabaseContext>(o => o.UseMySql("cityInfoDBConnectionString",
      //    MySqlServerVersion.LatestSupportedServerVersion, b => b.MigrationsAssembly("Entities")));
    }

    public static void ConfigureRepositoryWrapper(this IServiceCollection services)
    {
      services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
    }

    public static void ConfigureServiceLayerWrappers(this IServiceCollection services)
    {
      services.AddScoped<ICityService, CityService>();
    }
  }
}
