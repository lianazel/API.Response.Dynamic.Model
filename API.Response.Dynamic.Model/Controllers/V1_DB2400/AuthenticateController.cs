using API.Response.Dynamic.Model.Applications.DTOs;
using API.Response.Dynamic.Model.MailingMethods;
using API.Response.Dynamic.Model.SecurityMethods;
using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            user.UserName = userDto.Name;
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

                    // = = = = Attention : Inhibé ( car maintenant on envoie un mail ) = = = = =
                    // > On renvoie le Dto avec le Token calculé <
                    // result = this.Ok(userDto);

                    // - - - - - - Email Verification Debut - - - - - 
                    var confirmationlink = Url.Action(nameof(EmailVerification), "Authentication",
                        new { user.Id, userDto.Token }, Request.Scheme);

                    //  > Send mail <
                    //  ( Déclare un string pour intercepter un...
                    //    eventuel message d'erreur )
                    string ResMsge = string.Empty;
                    var email = new EmailService();
                    try
                    {
                        ResMsge = await email.Send(user.Email, "Verifier votre Email",
                         $"Click sur le lien pour vérifier le mail:{confirmationlink}");

                        return Ok("Enregistrement OK. Contrôler votre mail pour valider votre compte");
                    }
                    // > Quelque chose s'est mal passé => on renvoie...
                    //   ...le message d'erreur renvoyé par la méthode "Send" <
                    catch
                    {
                        userDto.ErrorMsge = ResMsge;

                    }
                }

                // > Le Anaomalie détectée à la phase d'inscription  <
                //   -> les possibilités d'erreurs sont :
                //      1/ adresse mail déjà existante,
                //      2/ mot de passe déjà utilisé,
                //      3/ mot de passe non conforme...
                //      4/ Echec authentification...
                //        ... du provider (méthode "Send" de la classe "EmailService").

                catch (Exception ex)
                {
                    // > Ne charge le message d'erreur que si la...
                    //   ...propriété  "ErrorMsge" du Dto est vide <
                    if (userDto.ErrorMsge == string.Empty)
                    {
                        StringBuilder sb = new StringBuilder();

                        // > Identificatioon du code message <
                        sb.Append("JWT_ERR_REGISTER_A1 - ");

                        // > Récupération du message d'erreur <
                        sb.Append(ex.Message);
                        userDto.ErrorMsge = sb.ToString();
                    }
                    
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

                foreach (char item in success.Errors.ToString())
                {
                    sb.Append(item);
                }
                userDto.ErrorMsge = sb.ToString();                  
                result = this.BadRequest(userDto);
            }

            return result;
        }


        [Route("emailverification")]
        [HttpGet]
        public async Task<IActionResult> EmailVerification(string userID, string Token)
        {
            // > Recherche de l'utilisateur à partir de son ID <
            //   ( Création d'une instance "User" )
            var user = await _userManager.FindByIdAsync(userID);
            if (user == null)
            {
                return BadRequest("JWT_EMAILCHEK_A1 - Invalid User ID");
            }

            // > Tentative de confirmation de l'email de l'utilisateur < 
            var result = await _userManager.ConfirmEmailAsync(user, Token);
            
            // > Si le membre booléen "Succeeded" est à vrai, on renvoie un message de succés <
            //  ( Sinon, on renvoie une BadRequest avec le message qui va bien )
            return result.Succeeded ? Ok("Email confirmé avec succés !") :
                    BadRequest("Echec confirmation Email.");
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
                        Login = user.Email,
                        Name = user.UserName,

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
