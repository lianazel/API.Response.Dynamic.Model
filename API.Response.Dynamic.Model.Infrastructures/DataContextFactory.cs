using API.Response.Dynamic.Model.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace API.Response.Dynamic.Model.Infrastructures
{
    /// <summary>
    ///  Usine à "context" 
    /// </summary>
    public  class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        // > La méthode "CreateDbContext" renvoie un objet de type "DataContext" <
        public DataContext CreateDbContext(string[] args) 
        {
            // *** Voir : 
            //  https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/


            // - - - - - - - - - -
            // = Message d'erreur "Could not load file or assembly 'API.Response.Dynamic.Model.DataMigration.dll,...
            //    ...Culture=neutral, PublicKeyToken=null'. Le fichier spécifié est introuvable."
            //  - - - - - - - - - -               
            // *** Voir : 
            // https://stackoverflow.com/questions/7859611/could-not-load-file-or-assembly-publickeytoken-null
            // https://stackoverflow.com/questions/48279531/could-not-load-file-or-assembly-project-name-culture-neutral-publickeytoken
            // https://www.nopcommerce.com/en/boards/topic/51785/could-not-load-file-or-assembly

            // - - - - - - - - - - - - 
            // (1) Construit un objet "config" pour manipuler "AppSettings" 
            // - - - - - - - - - - - - 
            // > On installe un "ConfigurationBuilder" pour indiquer au Framework...
            //   ...le nom du chemin où le fichier "appSettings.json" sera accessible.
            // > Ici, on indique au framework que le fichier "appSettings.json"  se trouve...
            //   ...dans le répertoire "Settings" du répertoire en cours.
            IConfiguration config = new ConfigurationBuilder()
           .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "Settings", "appSettings.json"))
           .Build();

            // - - - - - - - - - - - - 
            // (2) On indique à SqlServer la chaine de connexion ET le nom du projet de migration 
            // - - - - - - - - - - - - 
            // > Aprés avoir installé le NuGet "Microsoft.Entity.FrameworkCore.SqlServer"...
            //   ...on indique au Framework ou allez chercher la chaine de connexion <
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
            builder.UseSqlServer(config.GetConnectionString("PROD_BDD"), f => f.MigrationsAssembly("API.Response.Dynamic.Model.DataMigration"));

            // - - - - - - - - - - - - 
            // (3) On finalise la construction de l'instance "Context", de type "DataContext" 
            // - - - - - - - - - - - - 
            // > Attention au 2ème constructeur de la classe "DataContext" <
            //  ( On doit avoir : " public DataContext([NotNullAttribute] DbContextOptions options) : base(options)  " )
            DataContext context = new DataContext(builder.Options); 

            return context;
          
        }

    }
}
