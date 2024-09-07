namespace API.Responses.Dynamic.Model.Infrastructures.Services
{

    /// <summary>
    ///  Déporte dans une classe Dédié les types de paramètres 
    ///  Classe Non dérivable => amériore JIT (Just In Time ) 
    /// </summary>
    public sealed class ApiParamType 
    {
        // > Constructeur > 
        public ApiParamType()

        // - - - - - - - - - - - - 
        // ## Le constructeur initialisera les dictionnaires ci dessous 
        // - - - - - - - - - - - - 
        {
            // > Initialisation du dictionnaire pour insérer les opérateurs " AND " / " OR " <
            //   ( Attention : Maintenir les espaces AVANT et APRES )
            DicOpeLogAndOr[CS_OPELOG_AND] = " AND ";
            DicOpeLogAndOr[CS_OPELOG_OR] = " OR ";

            // > Initialisation du dictionnaire des messages d'erreurs 
            DicCodeErrorMsge[CS_ERR_OPECOMP] = "ERREUR-0955A - Opérateur de comparaison invalide.";
            DicCodeErrorMsge[CS_ERR_OPELOG] = "ERREUR-0965A - Opérateur logique invalide.";

        }
               


        // - - - - - - - - - - - - - - - - - -
        // > Version du programme  <
        // - - - - - - - - - - - - - - - - - -
        public string CS_VERSION = "V1.03 Beta";


        // - - - - - - - - - - - - - - - - - -
        // > Response Dynamic Model Maximum Records 
        // - - - - - - - - - - - - - - - - - -
        public int CS_MAXRECORDS = 150;

        // - - - - - - - - - - - - - - - - - -
        // > Type de valeurs renvoyées <
        // - - - - - - - - - - - - - - - - - -
        //   > INTeger <
        public string CS_TYP_INT = "INT";

        //   > DATe <
        public string CS_TYP_DAT = "DAT";

        //   > STRing <
        public string CS_TYP_STR = "STR";


        // - - - - - - - - - - - - - - - 
        // > Opérateurs logiques "&&" (AND) et "||" (OR) <
        // - - - - - - - - - - - - - - - 
        //   > AND <
        public string CS_OPELOG_AND = "&&";

        //   > OR <
        public string CS_OPELOG_OR = "||";


        // - - - - - - - - - - - - - - - - - - - - - 
        // > Dictionnaiore qui va contenir correspondance opérateurs logiques :  
        // - - - - - - - - - - - - - - - - - - - - - 
        // Exemple : -  "&&" sera associé à "AND"
        //           -  "||" sera associé à "OR". 
        public Dictionary<string, string> DicOpeLogAndOr = new Dictionary<string, string>();

        // - - - - - - - - - - - - - - - - - - - - - 
        // > Dictionnaiore qui va contenir les messages d'erreurs correspondant au codes erreur :  
        // - - - - - - - - - - - - - - - - - - - - - 
        // Exemple : -  "#ER-OPECOMP" sera associé à "ERREUR-0955A-Opérateur de comparaison ivalide"
        public Dictionary<string, string> DicCodeErrorMsge = new Dictionary<string, string>();


        // - - - - - - - - - - - - - - - - - -
        // Exemple de liste d'Item dans une liste de paramètre de condition ( N° personne = 5 ET Code rubrique = 3015 )
        //   <NOPPHN:==:5/> && <RUBL:==:3015/>;
        // - - - - - - - - - - - - - - - - - -
        // > Séparateur dans un Item <
        public string CS_SEPARATEUR = ":";
        // > Délimiteurs de type de paramètre    <
        public string CS_DELIMITEURS_DEB = "<";
        public string CS_DELIMITEURS_FIN = "/>";

        // - - - - - - - - - - - - - - - - - -
        // Exemple de liste d'Item dans une liste de paramètre de condition ( N° personne = 5 ET Code rubrique = 3015 )
        //   <NOPPHN:><:20230101;20230601/> ;   ( NOPPHN>20230101 and NOPPHN <20230101 )
        // - - - - - - - - - - - - - - - - - -
        // > Séparateur liste de valeur  <
        public string CS_SEPARATEUR_LIST_VALUE = ";";



        // - - - - - - - - - - - - - - 
        // Liste des codes Erreurs ( message d'erreurs stockées...
        // ...dans dictionnaire "DicCodeErrorMsge"
        // - - - - - - - - - - - - - - 
        //  > Erreur sur opérateur de comparaison  <
        public string CS_ERR_OPECOMP = "#ER-OPECOMP";

        //  > Erreur sur opérateur logique <
        public string CS_ERR_OPELOG = "#ER-OPELOG";

        // > Erreur Détectée lors de la récupération du paramètre <
        public string CS_ERROR = "ERREUR ANALYSE PARAMETRE";
    }
}
