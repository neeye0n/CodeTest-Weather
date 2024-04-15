using Microsoft.OpenApi.Models;
using Sorted.Weather.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(cfg => {
    cfg.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Rainfall Api", 
        Version = "v1",
        Description = "An API which provides rainfall reading data",
        Contact = new OpenApiContact
        {
            Name = "Sorted",
            Url = new Uri("https://www.sorted.com")
        }
    });
});

builder.Services.AddLogging();

var coreServiceConfig = new ServiceConfiguration();
builder.Configuration.GetSection("WeatherCoreConfig").Bind(coreServiceConfig);
builder.Services.RegisterCoreServices(coreServiceConfig);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rainfall Api"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
