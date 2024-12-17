namespace API.Response.Dynamic.Model.Applications.DTOs
{
    public class AuthenticateUserDto
    {
        #region properties

        // > Le Login Contiendra l'email <
        public string Login { get; set; }

        public string Password { get; set; }    

        public string Name { get; set; }

        // > Token calculé et retourné <
        public string Token   { get; set; }   

        // Contient message informatif <
        //   ( Erreur ou autre )
        public string InfoMsge { get; set; }

        #endregion

    }
}
