using MyApp.Application.Services;
using MyApp.Domain.Ports;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<IPropertiesService, PropertiesService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();