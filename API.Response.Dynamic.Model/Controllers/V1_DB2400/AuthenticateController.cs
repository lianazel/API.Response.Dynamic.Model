
using API.Response.Dynamic.Model.Applications.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Response.Dynamic.Model.Controllers.V1_DB2400
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthenticateController : ControllerBase
    {
        #region fields
        // - - - - - - - - - - - -
        // ### Authentication Debut ###
        // - - - - - - - - - - - -
        // > Déclare un "UserManager" <
        private UserManager<IdentityUser> _userManager = null;

        // > Déclare valriable pour extraire la clé de "Jwt:Key" <
        private string ElemKey;

        // > Tableau de strings utilisé pour la construction...
        //   ... de l'objet "host"  
        private string[] Elems = null;
        // - - - - - - - - - - - -
        // ### Authentication Fin ###
        // - - - - - - - - - - - -
        #endregion

        #region Constructor
        // > Constructeur <
        // Remarque : On recupère "userManager" par injection de dépendance <
        public AuthenticateController(UserManager<IdentityUser> userManager)
        {
            // > Chargement du Paramètre <
            this._userManager = userManager;

            // > On construit un objet "Host" pour pouvoir...
            //   ...manipuler un "appsettings.json" <
            using IHost host = Host.CreateDefaultBuilder(Elems).Build();
            IConfiguration config = host.Services.GetService<IConfiguration>();

            // > On extrait la valeur de la clé "JWT:Key" depuis le "AppSettings" <
            ElemKey = config.GetValue<string>("Jwt:Key");
        }
        #endregion

        #region methods

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthenticateUserDto DtoUser)
        {
            // > Par défaut, pessimiste <
            IActionResult result = this.BadRequest();


            // > On va verifier le profil via l'adresse mail et le mot de passe <
            // > L'idée, c'est de pouvoir s'authentifier avec le login qui...
            //   ... EST l'adresse mail. <
            // > Comme on fait une recherche avec l'adresse mail ( contenu dans...
            //   ...le login, on utilise la méthode "FindByEmailAsync" <

            // > Rappel : lorsque notre méthode "Login" est en "async",...
            //   ...il faut faire un "await" <
            var user = await this._userManager.FindByEmailAsync(DtoUser.Login);
                        
            if (user != null)
            {
                // > On va vérifier le password <
                var verif = await this._userManager.CheckPasswordAsync(user, DtoUser.Password);

                if (verif)
                {
                    // > On charge le DTO avec la calcul du Token renvoyé par la méthode...
                    //   ..."GenerateJwtToken" <
                    result = this.Ok(new AuthenticateUserDto()
                    {
                        Login = user.Email,
                        Name = user.UserName,
                       //Token = GenerateJwtToken(user),
                    });

                }

            }
            // > On retourne le résultat <
            return result;  
        }
        #endregion

    }
}
