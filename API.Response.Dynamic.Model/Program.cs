
using Microsoft.EntityFrameworkCore;
using API.Response.Dynamic.Model.Infrastructures.Data;
using API.Response.Dynamic.Model.Domain.Models;
using API.Response.Dynamic.Model.Infrastructures.Repositories;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using API.Response.Dynamic.Model.wwwroot.Swagger;
using System.Configuration;
using IoC.Features;
using System.Reflection;


// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// #####                     ---  Program.cs  ---                       #####
// #####                  **** Programme de D�marrage   ****            #####
// #####                           (.Net 6)                             #####
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=--=-=-=-=-
var builder = WebApplication.CreateBuilder(args);


// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// #####         **** Entity Framework Core .Net6  ****                     #####
// #####    Connxion du context ef Core � la chaine de connexion            #####
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// > Voir : 
// ==> https://stackoverflow.com/questions/68980778/config-connection-string-in-net-core-6
builder.Services.AddDbContext<DataContext>(options =>
{
// > Pour la prod <
//options.UseSqlServer(builder.Configuration.GetConnectionString("PROD_DEV"));

options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["PROD_BDD"],
    sqlServerOptionsAction: SqlOptions =>
    {
        // > Impl�mentation r�sulient EF Core => SQL Connection <
        //   ( Reconnection � la base de donn�es en cas d'�chec )
        SqlOptions.EnableRetryOnFailure(
            maxRetryCount: 2,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    });

});


// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// #####                            **** Entity Framework Core .Net6  ****                     #####
// ##### On met en place le lien entre l'interface et la classe qui respecte ce contrat ici.   #####
// ##### "builder.Services.AddScoped<IParamRepository, DefaultParamRepository>();"             #####
// #####                                                                                       #####
// ##### Le Framework pourra alors effectuer l'injection d'une instance de la classe...        #####
// #####  "DefaultParamRepository" qui respecte le contrat de l'interface...                   #####
// ##### ... "IParamRepository".                                                               #####
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
builder.Services.AddScoped<IParamRepository, DefaultParamRepository>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// - - - - - - - - - - - - - - -
// Versionning API 
// Voir :
//  https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#12-versioning
// - - - - - - - - - - - - - - -

// - - - - - - - - - - - - - - - - - - - - 
// Configuration du VERSIONNING de l'API ( DEBUT ) 
// - - - - - - - - - - - - - - - - - - - - 

// - - - - - - - - - - - - - - -
// Versionning API 
// Voir :
//  https://mohsen.es/api-versioning-and-swagger-in-asp-net-core-7-0-fe45f67d8419
// - - - - - - - - - - - - - - -

builder.Services
    .AddApiVersioning(

    options =>
    {
        // > indique si une version par d�faut est suppos�e lorsqu'un client ne...
        //   ...fournit pas de version d'API.
        options.AssumeDefaultVersionWhenUnspecified = true;

        // > Liste les versions d'API disponibles <
        options.ReportApiVersions = true;

        // > D�finit une version par d�faut  <
        options.DefaultApiVersion = new ApiVersion(3.0);
    } )
    
    
    .AddApiExplorer(options =>
    {
        // Add the versioned API explorer, which also adds IApiVersionDescriptionProvider service
        // note: the specified format code will format the version as "'v'major[.minor][-status]"
        options.GroupNameFormat = "'v'VVV";

        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
        // can also be used to control the format of the API version in route templates
        options.SubstituteApiVersionInUrl = true;
    });


// > Configuration du builder avec les classes "SwaggerGenOptions" et "ConfigureSwaggerOptions" <
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    // Ajouter un filtre d'op�ration personnalis� qui d�finit les valeurs par d�faut
    options.OperationFilter<SwaggerDefaultValues>();
});


// > R�c�p�ration de la documentation install�e au niveau du contr�leur <
builder.Services.AddSwaggerGen(options =>
{
    // > Ajouter un filtre d'op�ration personnalis� qui d�finit les valeurs par d�faut <
    options.OperationFilter<SwaggerDefaultValues>();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


// - - - - - - - - - - - - - - - - - - - - 
// Configuration du VERSIONNING de l'API ( FIN ) 
// - - - - - - - - - - - - - - - - - - - - 


// > Construction de l'objet builder <
var app = builder.Build();



// - - - - - - - - - - - - - - - - - - - - 
//> Configiration du Swagger pour le d�v�loppement <
//   [ Configuration du VERSIONNING de l'API ( DEBUT ) ]
// - - - - - - - - - - - - - - - - - - - - 
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        // Build a swagger endpoint for each discovered API version
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
    
}
// - - - - - - - - - - - - - - - - - - - - 
//> Configiration du Swagger pour le d�v�loppement <
//   [ Configuration du VERSIONNING de l'API ( Fin ) ]
// - - - - - - - - - - - - - - - - - - - - 




// > Crypatage des donn�es �chang�es entre le l'API et le client <
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
