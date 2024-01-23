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
builder.Services.ConfigureRepositoryWrapper();
builder.Services.ConfigureServiceLayerWrappers();

// Mapster
TypeAdapterConfig<City, CityDto>.NewConfig().Map(dest => dest.CityLanguages, src => src.CityLanguages.Select(x => x.Language)).Map(dest => dest.CityId, src => src.CityId);
TypeAdapterConfig<CityDto, City>.NewConfig();
TypeAdapterConfig<CityForUpdateDto, City>.NewConfig();
// Mapning herover bevirker, at man får LanguageName med ud, når man konverterer fra 
// City Objekter(er) til CityDTO Objekt(er)
TypeAdapterConfig<Country, CountryDto>.NewConfig().Map(dest => dest.CountryID, src => src.CountryID);
TypeAdapterConfig<Language, LanguageDto>.NewConfig().Map(dest => dest.CityLanguages, src => src.CityLanguages.Select(x => x.City));
TypeAdapterConfig<CityLanguage, CityLanguageDto>.NewConfig().Map(dest => dest.CityId, src => src.CityId).Map(dest => dest.LanguageId, src => src.LanguageId).
   Map(dest => dest.City, src => src.City);

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
