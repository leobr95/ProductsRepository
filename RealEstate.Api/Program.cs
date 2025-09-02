using System.Globalization;
using Microsoft.AspNetCore.Localization;
using RealEstate.Infrastructure.DI;   
using Swashbuckle.AspNetCore.Filters; 
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Localization (para IStringLocalizer<SharedResources>)
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Controllers + Swagger (Swashbuckle)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();         // 👈 activa [SwaggerOperation], [SwaggerResponse], etc.
    c.ExampleFilters();            // 👈 habilita providers de ejemplos

    // (opcional) info del doc
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RealEstate API",
        Version = "v1",
        Description = "API for RealEstate with sample examples in Swagger"
    });

    // XML comments para summary/param/returns (asegúrate de generar XML en el .csproj)
    var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
}

);
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowLocalFront", p =>
        p.WithOrigins("http://localhost:3000")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

// Capas de aplicación e infraestructura (proveedor Memory/SqlServer + validaciones)
builder.Services.AddApplicationCore();
builder.Services.AddDataProvider(builder.Configuration);

var app = builder.Build();

// Localización de la request (idioma desde appsettings: "Language": "en" | "es")
var lang = builder.Configuration.GetValue<string>("Language")?.ToLowerInvariant() ?? "en";
var defaultCulture = lang.StartsWith("es") ? "es-ES" : "en-US";
var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("es-ES") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalFront");
app.MapControllers();

app.Run();
