using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Response.Dynamic.Model.SecurityMethods
{/// <summary>
///  > Génération du Token qui sera...
///     > renvoyé à l'appelant pour Authentification <
/// </summary>
    public class JwtTokenGeneration
    {
        #region Fields
        // > Déclare valriable pour extraire la clé de "Jwt:Key" <
        private string? ElemKey;


        // > Tableau de strings utilisé pour la construction...
        //   ... de l'objet "host"  
        private string[] Elems = null;

        // > Utilisée pour la génération du Token <
        private IdentityUser _user;

        #endregion


        #region constructor
        public  JwtTokenGeneration(IdentityUser user)
        {
            _user=user;


            // > On construit un objet "Host" pour pouvoir...
            //   ...manipuler un "appsettings.json" <
            using IHost host = Host.CreateDefaultBuilder(Elems).Build();

            IConfiguration config = host.Services.GetService<IConfiguration>();

            // > On extrait la valeur de la clé "JWT:Key" depuis le "AppSettings" <
            ElemKey = config.GetValue<string>("Jwt:Key");

        }
        #endregion

        #region methods

        // > Méthode public de génération du Token <
        public string RunTwtGeneration()
        {
            return GenerateJwtToken();
        }

        /// <summary>
        ///  Méthode privée pour la génération du Token 
        /// </summary>
        /// <returns></returns>
        private string GenerateJwtToken()
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
                     new Claim("Id",_user.Id),
                     new Claim(JwtRegisteredClaimNames.Sub,_user.Email) ,
                     new Claim(JwtRegisteredClaimNames.Email,_user.Email),
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
