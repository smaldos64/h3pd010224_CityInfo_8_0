using CityInfo_8_0_Server.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using NLog;

using Entities;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Configuration;
using Entities.DataTransferObjects;
using Entities.Models;
using Mapster;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// LTPE Added below
//var nLogConfigPath = string.Concat(Directory.GetCurrentDirectory(), "/nlog.config");
//if (File.Exists(nLogConfigPath)) { LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config")); }
//Configuration = configuration;
try
{
  LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
}
catch (Exception Error)
{
Console.WriteLine(Error.Message);
}
//var Logger = NLog.LogManager.GetCurrentClassLogger();

builder.Services.ConfigureCors(); 
builder.Services.ConfigureIISIntegration(); 
builder.Services.ConfigureLoggerService(); 

builder.Services.ConfigureMsSqlContext(builder.Configuration);

//string connectionString = Environment.GetEnvironmentVariable("cityInfoDBConnectionString");
//builder.Services.AddDbContext<DatabaseContext>(o => o.UseSqlServer(connectionString, x => x.MigrationsAssembly("Entities")));
//DatabaseContext sqlDbContext = new(builder.Configuration["ConnectionStrings:cityInfoDBConnectionString"]);
//string ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Inject a DbContextOptions instance later
//builder.Services.AddSingleton<DbContextOptions>(options =>
//{
//  var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
//  optionsBuilder.UseSqlServer(ConnectionString);
//  return optionsBuilder.Options;
//});

//string ConnectionString = builder.Configuration.GetConnectionString("cityInfoDBConnectionString");
//builder.Services.AddSingleton<DatabaseContext>().AddDbContext<DatabaseContext>(Options =>
//{
//  var OptionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
//  OptionsBuilder.UseSqlServer(ConnectionString);
//  return OptionsBuilder.Options;
//});
//builder.Services.AddSingleton<DbContextOptions>(options =>
//{
//  var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
//  optionsBuilder.UseSqlServer(ConnectionString);
//  return optionsBuilder.Options;
//});

builder.Services.ConfigureRepositoryWrapper();
builder.Services.ConfigureServiceLayerWrappers();

// Mapster
UtilityService.SetupMapsterConfiguration();
// LTPE added above

//Json 
//builder.Services.AddControllers().AddJsonOptions(x =>
//                 x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// LTPE added above

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
else // LTPE
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }