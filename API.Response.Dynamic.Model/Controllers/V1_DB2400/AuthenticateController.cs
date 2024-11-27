using API.Response.Dynamic.Model.Applications.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        // > Constructeur (On recupère "userManager" par injection de dépendance) <
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
                        Login  = user.Email,
                        Name   = user.UserName,
                        // > Appelle la méthode "GenerateJwtToke" qui se charge...
                        //   ...de claculer un nouveau Token < 
                        Token = GenerateJwtToken(user),
                    });

                }

            }
            // > On retourne le résultat <
            return result;  
        }

        private string GenerateJwtToken( IdentityUser user)
        {

            // > On définit le Jwt token qui sera responsable...
            // ...de la création de notre Token <
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // > On récupère notre code secret depuis "app.settings" <
            // ( Attention : Travaille ici avec "UTF8" car dans la méthode...
            //   ..."AddCustomAuthentication" de la classe "SecurityMethods"...
            //   ...dans laquelle on décrit le TOKEN, on déclare de l' "UTF8" ).

            // ( Il est important de garder les mêmes paramètres, sinon le...
            //   ...Framework ne pourra PAS comparer ).
            var key = Encoding.UTF8.GetBytes(ElemKey);

            // -----------------------------
            // --- Description du Token ----
            // -----------------------------
            // > On a besoin d'utiliser des "claims" (=> "Réclamation" en français)...
            // ...qui sont des propriétés dans notre Token qui donne des...
            // ...informations à propos du Token qui appartient à l'utilisateur...
            // ...et on a donc des informations telles que l'ID de l'utilisateur,...
            // ...le nom de l'utilisateur, son adresse mail...
            // ...Le bon côtéest que ces informations sont générées par ...
            // ...notre serveur et notre framework d'identité qui sont valides et fiables. <

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                 {
                     new Claim("Id",user.Id),
                     new Claim(JwtRegisteredClaimNames.Sub,user.Email) ,
                     new Claim(JwtRegisteredClaimNames.Email,user.Email),
                     // > Le JTI est utilisé pour notre Token <
                     new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                 }),

                // > La durée de vie du jeton doit être plus...
                //   ...courte et utiliser un jeton d'actualisation pour que...
                //   ...l'utilisateur reste connecté. <

                // > Le Token va avoir un durée de trois minutes <
                Expires = DateTime.UtcNow.AddMinutes(3),

                // > On ajoute ici les informations sur l'algorithme de chiffrement...
                //   ...qui seront utilisées pour déchifrer notre jeton <
                SigningCredentials = new SigningCredentials(

                // > Attention : travaille ici avec "SymmetricSecurityKey" car...
                //   ...dans la méthode "addCustomAuthentication" de la classe...
                //   ..."SecurityMethods" dans la quelle on décrit le "TOKEN", ...
                //   ...on déclare de l'"SymmetricSecurityKey". <
                // > Il est important de garder les mêmes paramètres, sinon...
                //   ...le FrameWork ne pourra PAS comparer <
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var JwtToken = jwtTokenHandler.WriteToken(token);   

            return JwtToken;    

        }

        #endregion

    }
}
