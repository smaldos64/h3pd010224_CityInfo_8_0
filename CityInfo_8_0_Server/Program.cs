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
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

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

//builder.Services.ConfigureCors(); 
builder.Services.ConfigureIISIntegration(); 
builder.Services.ConfigureLoggerService(); 

builder.Services.ConfigureMsSqlContext(builder.Configuration);

builder.Services.ConfigureRepositoryWrapper();
builder.Services.ConfigureServiceLayerWrappers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed((host) => true));
});

// Mapster
//UtilityService.SetupMapsterConfiguration();
// Med den nuværende konfiguration af DTO klasser er det ikke nødvendigt at
// sætte noget overhovedet med hensyn til Mapster !!!
// LTPE added above

//Json 
//builder.Services.AddControllers().AddJsonOptions(x =>
//                 x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// LTPE added above

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c => {
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.IgnoreObsoleteActions();
    c.IgnoreObsoleteProperties();
    c.CustomSchemaIds(type => type.FullName);
});

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

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }