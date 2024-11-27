// > Authentication JWT <
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


namespace API.Response.Dynamic.Model.SecurityMethods
{
    public static class SecurityMethods
    {
        #region properties 
        // > Déclare clé pour la construction du Token <
        // private static string MyKeyJwt { get; set; }
        private static string MyKeyJwt;

        // > Cré une variable "args" de type tableau de string nullable (?) <
        // ( pour " var builder = WebApplication.CreateBuilder(args) )
        // ( Remarque : on peut l'appeler autrement que "args" si on veut )
        private static string[]? args { get; set; }

        #endregion
        /// <summary>
        /// Construction du processus d'authentification JWT
        /// </summary>
        /// <param name="_builder"></param>
        /// <returns></returns>
        public static object AddCustomAuthentication( object _builder)
        {
            // > 1/ Création d'un "configurationBuilder" <
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            // > 2/ Création d'un objet "Configuration" à partir du...
            //      ..."configurationBuilder" <
            IConfiguration configuration = configurationBuilder.Build();
                        
           
            // > 3/ On déclare une variable "builder" de type...
            //      ... "WebAplicationBuilder" <
            var builder = WebApplication.CreateBuilder(args);

            // > 4 Si le paramètre transmis est non nul, alors on récupère son contenu <
            if ( _builder is not null)
            {
                // > On Parse "_builder" avec la classe "WebApplicationBuilder" <
                builder = (WebApplicationBuilder) _builder; ;
            }

            // > 5/ Récupération de la clé d'authentification <
            //     ( On récupère la clé depuis "appsettings.json" )
            MyKeyJwt = string.Empty;    
            MyKeyJwt = builder.Configuration.GetSection("Jwt")["key"];

            
            // =-=-=-=-=-=-=-=-=-=-
            // =-=-=- DEBUT Phase Validation du <Json Web Token> =-=-=-=
            // =-=-=-=-=-=-=-=-=-=-
            //  > On détermine ici quelle est la façon de communiquer des Headers...
            //    ...dans la partie authentification.

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                // > C'est ici que l(on configure jwt avec tous les paramètres.<
                // > Par exemple, il va nous falloir une clé unique...
                //   ...d'authentification qui va être sur notre site web.

                // Il va falloir tester toute une partie pour savoir si...
                // ...l'utilisateur est bien connecté, si l'audience est bien spécifiée.
                {
                    // > On sauvegarde le Tpken pour pouvoir l'utiliser plus tard <
                    options.SaveToken  = true;

                    // > Paramètres de validation du Token <
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        // > On va paramétrer notre Token avec la clé.
                        // > Celle ci  est encodée (=>"SymmetricSecurityKey") <
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(MyKeyJwt)),
                        
                        // > En activant "ValidateAudience", on s'assure que le jeton ne sera...
                        //   ...accépté que s'il contient l'audience spécifiée <
                        ValidateAudience = false,
                        
                        // > Verifie que l'emetteur du Jeton correspond à ce que cette API attend <
                        //    ( On vérifie que le Jeton est émis par une source de confience )
                        ValidateIssuer = false,

                        // > Un jeton d'acteur est un jeton JWT qui représente un utilisateur...
                        //   ...ou une entité ou une entité agissant au nom d'un autre utilisateur...
                        //   ... entité <
                        ValidateActor = false,
                        
                        // > On dit que le Token à une durré de vie limitée <
                        ValidateLifetime = true

                                          
                    };

                }
            });

            // =-=-=-=-=-=-=-=-=-=-
            // =-=-=- FIN Phase Validation du <Json Web Token> =-=-=-=
            // =-=-=-=-=-=-=-=-=-=-

            return builder;

        }
    }
}


