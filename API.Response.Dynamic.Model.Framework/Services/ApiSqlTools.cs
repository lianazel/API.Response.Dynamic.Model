using System.Text.RegularExpressions;

namespace API.Response.Dynamic.Model.Infrastructures.Services
{
    /// <summary>
    ///   - Vérification basique  de la requête SQL : La requête SQL doit...
    ///   ...démarrer par un 'SELECT' et avoir une clause 'WHERE'  
    ///   - Je peux ajouter d'autres contrôles
    ///   ( Via desExpressions Régulières ) 
    /// </summary>
    public class ApiSqlTools

    {
        #region properties 
        // > Chaine SQL <
        string SqlItem;
       
        #endregion

        #region methods 
        /// <summary>
        ///  > Construsteur < 
        /// </summary>
        /// <param name="_SqlItem"></param>
        public ApiSqlTools(string _SqlItem)
        {
            
            // > Reception Requête SQL <
            SqlItem = _SqlItem.ToUpper();
            
        }

        /// <summary>
        ///   Effectue des contrôles sur la chaine SQL
        ///     ( Recoit un tableau de chaine tel que ("CHECKSELECT","CHECKWHERE") 
        /// </summary>
        /// <param name="_SqlCheck"></param>
        /// <returns></returns>
        public string SqlFunctionsCtl(string[] _SqlCheck)
        {
            // > Dictionnaire d'aiguiilage pour l'appel des fonctions <
            //    - Les fonctionns recoivent une chaine  en paramètres,
            //    -                Retournent un booléen.
            Dictionary<string, Func<string, bool>> DictsqlTestFunctions = new Dictionary<string, Func<string, bool>>();

            // > Dictionnaire pour la gestion des messages d'erreurs <
            Dictionary<string, string> DictErrorMsge = new Dictionary<string, string>();
                     
            // > Resultat de l'analyse <
            bool IsMatch;
            IsMatch = false;

            // > Charge dictionnaire avec littéral "CHECKSELECT" qui apelle la fonction "CheckSelect" <
            DictsqlTestFunctions.Add("CHECKSELECT", CheckSelect);
            DictErrorMsge.Add("CHECKSELECT", "ERR-APISQL001 query must be of type 'select' ");

            // > Charge dictionnaire avec littéral "CHECKWHERE" qui apelle la fonction "CheckSWhere" <
            DictsqlTestFunctions.Add("CHECKWHERE", CheckSWhere);
            DictErrorMsge.Add("CHECKWHERE", "ERR-APISQL002 query must contain a 'Where' clause");

            // > Parcour la liste des contrôles SQL à faire <
            foreach (string item in _SqlCheck)
            {
                // > Parcour chaque poste du dictionnaire  d'aiguillage appel de fonctions <
                foreach (var entry in DictsqlTestFunctions)
                {
                    // > Si le contrôle demandé est égal à la clé <
                    if (item.Contains(entry.Key))
                    {
                        // > Appelle la fonction avec en paramètre la requête SQL à contrôler  <
                        IsMatch = entry.Value(SqlItem);

                        // > Renvoie le message d'erreur correspondant au contrôle <
                        if (IsMatch == false)
                        {                            
                            return DictErrorMsge[entry.Key];
                        }


                    }

                }
            }

            // - - - - - - -
            // > Functions "CheckSelect" <
            //   ( Exemple d'Expression Régulières ) 
            // - - - - - - -
            bool CheckSelect(string sql)
            {
                string patten = @"^\SELECT";

                Match match = Regex.Match(sql, patten, RegexOptions.IgnoreCase);

                // > La requête SQL contient est bien de type select <
                if (match.Success == true)
                {
                    return true;
                }

                return false;
            }

            // - - - - - - -
            // > Functions "CheckSWhere" <
            //   ( Exemple d'Expression Régulières ) 
            // - - - - - - -
            bool CheckSWhere(string sql)
            {
                string patten = @"\bWHERE\b";

                Match match = Regex.Match(sql, patten, RegexOptions.IgnoreCase);

                // > La requête SQL contient bien une clause "Where"  <
                if (match.Success == true)
                {
                    return true;
                }

                return false;
            }

            // > Aucun message renvoyé : tout est OK <
            return "";

        }

        #endregion
    }

}
