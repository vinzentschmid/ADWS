using DAL.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using Services;
using System.Text.Json.Serialization;
using Utilities.Logging;
using Utils;
using Utils.Consul;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<TimeScaleContext>();
builder.Services.AddSingleton<ISettings, DataSettings>();
builder.Services.AddSingleton<ISettingsHandler, ConsulSettingsHandler>();
builder.Services.AddSingleton<IServiceStart, ServiceStart>();
builder.Services.AddSingleton<IAquariumLogger, AquariumLogger>();
builder.Services.AddSingleton<IGlobalService, GlobalService>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;

}
).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.IgnoreNullValues = true;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());
});


builder.Services.AddControllers().AddNewtonsoftJson(opts => opts.SerializerSettings.Converters.Add(new StringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});



builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Aquarium Data API",
        Version = "v1",
        Description = "API for Aquarium Data"
    });

    c.UseAllOfForInheritance();
    //c.UseOneOfForPolymorphism();

}
            ); ;

builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddSwaggerDocument(settings =>
{
    settings.Title = "Aquarium Data API v1";
});

var app = builder.Build();
app.UseOpenApi();
app.UseSwaggerUi();

//app.UseHttpsRedirection();


using (var serviceScope = app.Services.CreateScope())
{
    var serviceProvider = serviceScope.ServiceProvider;

    ISettings Settings = serviceProvider.GetRequiredService<ISettings>();
    ISettingsHandler SettingsHandler = serviceProvider.GetRequiredService<ISettingsHandler>();
    await SettingsHandler.Load();
    IAquariumLogger AquariumLogger = serviceProvider.GetRequiredService<IAquariumLogger>();
    await AquariumLogger.Init();


    IServiceStart start = serviceProvider.GetRequiredService<IServiceStart>();
    await start.Start();
}

//builder.Host.UseSerilog(log);

app.UseAuthorization();

app.MapControllers();


app.UseCors("CorsPolicy");




app.Run();
