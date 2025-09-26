using MyApp.Application.Services;
using MyApp.Domain.Ports;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration["AllowedOrigins"] ?? ""; 
var origins = allowedOrigins
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontends", policy =>
    {
        policy.WithOrigins(origins)              
              .AllowAnyMethod()                 
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MongoDbContext>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var databaseName = "millions";
    return new MongoDbContext(connectionString, databaseName);
});

builder.Services.AddScoped<IPropertiesRepository, MongoPropertiesRepository>();
builder.Services.AddScoped<IOwnerRepository, MongoOwnerRepository>();
builder.Services.AddScoped<IPropertyTraceRepository, MongoPropertyTraceRepository>();
builder.Services.AddScoped<IPropertiesService, PropertiesService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("Frontends");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();