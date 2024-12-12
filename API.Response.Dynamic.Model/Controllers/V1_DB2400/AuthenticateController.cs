using API.Response.Dynamic.Model.Applications.DTOs;
using API.Response.Dynamic.Model.SecurityMethods;
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

            
        }
        #endregion

        #region methods

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AuthenticateUserDto userDto)
        {

            // > Par défaut, pessimiste <
            IActionResult result = this.BadRequest();

            // ---------------------------
            // ## DEBUT - Construit l'objet "user" ##
            // ---------------------------
            var user = new IdentityUser(userDto.Login);
            user.Email = userDto.Login;
            user.UserName  = userDto.Name;
            // ---------------------------
            // ## DEBUT - Construit l'objet "user" ##
            // ---------------------------

            // > Tentative d'écriture en Base du nouveau User <
            var success = await this._userManager.CreateAsync(user, userDto.Password);

            if (success.Succeeded)
            {
                try
                {
                    // > On instancie la classe "JwtTokenGeneration" <
                    var GenerateToken = new JwtTokenGeneration(user);

                    // > On récupère le Token calculé par la méthode "RunTwtGeneration" <
                    userDto.Token = GenerateToken.RunTwtGeneration();
                    // > On renvoie le Dto avec le Token calculé <
                    result = this.Ok(userDto);                                      
                }

                // > Le Anaomalie détectée à la génératioon du Token  <
                catch (Exception ex)
                {
                    StringBuilder sb = new StringBuilder();

                    // > Identificatioon du code message <
                    sb.Append("JWT_ERR_REGISTER_A1 - ");
                    
                    // > Récupération du message d'erreur <
                    sb.Append(ex.Message);
                    // userDto.ErrorMsge = sb.ToString();
                    // ( Utilisation de "nameof()" => plus rapide que "sb.ToString()" )
                    userDto.ErrorMsge = nameof(sb);

                    // > On renvoie le Dto avec le Message d'erreur <
                    result = this.BadRequest(userDto);
                }

                finally
                { }
            }


            // > Quelque chose s'est mal passé <
            //   - Mot de passe incorrect ( ne respecte PAS les règles prédéfinies ),
            //                  ou
            //   - Un utilisateur avec ce login existe déjà...
            else
            { 
            
                StringBuilder sb = new StringBuilder();

                // > Identificatioon du code message <
                sb.Append("JWT_ERR_REGISTER_A2 - ");

                foreach ( char item in success.Errors.ToString())
                {
                    sb.Append(item);

                }
                // > Utilisation de "nameof()" => plus rapide que "sb.ToString()" <
                // userDto.ErrorMsge = sb.ToString();  
                userDto.ErrorMsge = nameof(sb);  
                result = this.BadRequest(userDto);
            }  

            return result;      
        }

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
                    // > On instancie la classe "JwtTokenGeneration" <
                    var GenerateToken = new JwtTokenGeneration(user);

                    // > On charge le DTO avec la calcul du Token renvoyé par la méthode...
                    //   ..."GenerateJwtToken" <
                    result = this.Ok(new AuthenticateUserDto()
                    {
                        Login  = user.Email,
                        Name   = user.UserName,

                        // > On récupère le Token calculé par la méthode "RunTwtGeneration" <
                        Token = GenerateToken.RunTwtGeneration()
                    });                                       
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    // > Identificatioon du code message <
                    sb.Append("JWT_ERR_LOGIN_A1 - Adresse Mail ou mot de passe invalide");
                    result = this.BadRequest(new AuthenticateUserDto()
                    {
                        Login = user.Email,
                        Name = user.UserName,
                        // > Appelle la méthode "GenerateJwtToke" qui se charge...
                        //   ...de claculer un nouveau Token < 
                        Token = null,

                        // > Utilisation de "nameof()" => plus rapide que "sb.ToString()" <
                        // ErrorMsge = sb.ToString()
                        ErrorMsge = nameof(sb)
                    });
                }
            }
            // > On retourne le résultat <
            return result;  
        }


        #endregion

    }
}
