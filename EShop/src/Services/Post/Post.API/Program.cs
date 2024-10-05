using System.Collections;
using BuildingBlock.Exceptions.Handlers;
using BuildingBlock.Middlewares;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var mongoConnectionString =
    builder.Configuration["MongoSettings:ConnectionString"];
var databaseName = builder.Configuration["MongoSettings:DatabaseName"];
var collectionName = builder.Configuration["MongoSettings:CollectionName"];

// log to file
builder.Host.UseSerilog((context, services, configuration) => {
  // Define the log file name pattern with timestamp
  var logFilePath = $"Logs/log.txt";

  configuration.MinimumLevel.Override(
      "Microsoft",
      LogEventLevel.Warning); // Set Microsoft logs to Warning level
  configuration.MinimumLevel.Override(
      "Microsoft.Hosting.Lifetime",
      LogEventLevel.Error); // Suppress application start logs
  configuration.MinimumLevel.Override(
      "Microsoft.AspNetCore",
      LogEventLevel.Warning); // Suppress HTTP request logs

  configuration.WriteTo.Console(); // Log to console
  configuration.WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day,
                             restrictedToMinimumLevel: LogEventLevel
                                 .Information); // Write logs to file
});
builder.Services
    .AddTransient<ICustomExceptionHandler, CustomExceptionHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at
// https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
  app.UseMiddleware<RequestResponseLoggingMiddleware>(
      mongoConnectionString, databaseName, collectionName);
  app.UseMiddleware<ExceptionHandlingMiddleware>(); // register handle exception
}

app.UseAuthorization();

app.MapControllers();

app.Run();
