
using Microsoft.EntityFrameworkCore;
using API.Response.Dynamic.Model.Infrastructures.Data;
using API.Response.Dynamic.Model.Domain.Models;
using API.Response.Dynamic.Model.Infrastructures.Repositories;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using API.Response.Dynamic.Model.wwwroot.Swagger;
using System.Reflection;
using Microsoft.OpenApi.Models;
using API.Response.Dynamic.Model.SecurityMethods;
using Microsoft.AspNetCore.Identity;


// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// #####                     ---  Program.cs  ---                       #####
// #####                  **** Programme de Démarrage   ****            #####
// #####                           (.Net 6)                             #####
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=--=-=-=-=-
var builder = WebApplication.CreateBuilder(args);


// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// #####         **** Entity Framework Core .Net6  ****                     #####
// #####    Connxion du context ef Core à la chaine de connexion            #####
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
            // > Implémentation résulient EF Core => SQL Connection <
            //   ( Reconnection à la base de données en cas d'échec )
            SqlOptions.EnableRetryOnFailure(
                maxRetryCount: 2,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        });

    });


// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// #####                            **** Entity Framework Core .Net8  ****                     #####
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
        // > indique si une version par défaut est supposée lorsqu'un client ne...
        //   ...fournit pas de version d'API.
        options.AssumeDefaultVersionWhenUnspecified = true;

        // > Liste les versions d'API disponibles <
        options.ReportApiVersions = true;

        // > Définit une version par défaut  <
        options.DefaultApiVersion = new ApiVersion(3.0);
    })


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


// Authentication Swagger - Error Message "TypeError: Failed to execute 'fetch' on 'Window': Invalid name"
// voir https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/704

// > Récupération de la documentation installée au niveau du contrôleur <
builder.Services.AddSwaggerGen(options =>
{
    // > Ajouter un filtre d'opération personnalisé qui définit les valeurs par défaut <
    options.OperationFilter<SwaggerDefaultValues>();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // > Ajout Authentification JWT 1/2 <
    options.AddSecurityDefinition("Bearer ", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "En-tête d’autorisation JWT utilisant le schéma Bearer.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey

    });

    // > Ajout Authentification JWT 2/2 <
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer "
                },
                Scheme = "oauth2" , Name = "Bearer " ,

                In = ParameterLocation.Header,
            },

            new List<string>() 
            
        }
    });

});
// - - - - - - - - - - - - - - - - - - - - 
// Configuration du VERSIONNING de l'API ( FIN ) 
// - - - - - - - - - - - - - - - - - - - - 



// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// ###      *** JWT .Net 8 ***        #### 
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
SecurityMethods.AddCustomAuthentication(builder);


// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
// ###               *** JWT .Net 8 ***              ###
// ###    > Configurations options mot de passe      ###
// ###    > Association du contexte                  ###
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // > On demande à se ce que le mot de passe...
    //   ...contienne des chiffres <
    options.Password.RequireDigit = true;

    // > On demande à se ce que le mot de passe...
    //   ...contienne des majuscules <
    options.Password.RequireUppercase = true;

    // > On demande à se ce que le mot de passe...
    //   ...contienne minimum 12 caractères <
    options.Password.RequiredLength = 12;

    // > Demande ici la saisie d'une adresse Mail <
    options.SignIn.RequireConfirmedEmail = true;    

//  Association avec notre Context pour effectuer l'authentification 
}).AddEntityFrameworkStores<DataContext>();


// > Construction de l'objet builder <
var app = builder.Build();



// - - - - - - - - - - - - - - - - - - - - 
//> Configiration du Swagger pour le dévéloppement <
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
//> Configiration du Swagger pour le dévéloppement <
//   [ Configuration du VERSIONNING de l'API ( Fin ) ]
// - - - - - - - - - - - - - - - - - - - - 

// - - - - - - - - - - - - - - -
// > Création automatique de la base de données <
// - - - - - - - - - - - - - - -
using (var scope = app.Services.CreateScope())
{

    // -- Chargement des tables définies dans le contexte - 
    var dbContext = scope.ServiceProvider.GetService<DataContext>();  

    // -- Création des tables --
    dbContext.Database.EnsureCreated();

}
 


// > Crypatage des données échangées entre le l'API et le client <
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
