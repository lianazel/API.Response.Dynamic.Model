using System.Reflection;
using System.Reflection.Emit;
using System.Text;


namespace API.Response.Dynamic.Model.Infrastructures.Services
{
    /// <summary>
    ///  > Renvoie une collection de models de données dynamiques  <
    /// </summary>
    public class ApiDynamicResultsFromSql
    {
        #region properties

        // - - - -
        // Gestion message Retour Traitement 
        // - - - -
        // > Traitement : si message d'erreur à renvoyer  <
        string MsgJobStatus;

        // > String Builder pour construction message erreur <
        StringBuilder sb = new StringBuilder();

        #endregion

        #region methods

        // > Constructeur <
        public ApiDynamicResultsFromSql()
        { }




        /// <summary>
        ///  > Renvoie une collection de modèles de données dynamiques <
        ///     ( Le deuxième paramètre coorespond à la liste des enregistrements )
        /// </summary>
        /// <param name="_DynamicModel">          Modèle de type "TpeBuilder" </param>
        /// <param name="_RowsDictColumnsValue">  List d'instances de modèles de données dynamiques 
        /// <returns> string </returns>
        public string ReturnListObjects(TypeBuilder _DynamicModel, List<Dictionary<string, object>> _RowsDictColumnsValue, out List<object> OutListInstanceMapped)
        {
            // > Initialise Message de compte rendu  <
            //  ( Note : si renvoyé à vide ==> traitement OK )
            MsgJobStatus = string.Empty;

            OutListInstanceMapped = new List<object>();

            // > Déclare une liste qui va recevoir un dictionnaire pour chaque ligne < 
            //   ( Dictionnaire : Nom Colonne / Valeur Associée )
            Dictionary<string, object> RowDictColumnsValue = new Dictionary<string, object>();

            // > Parcourt chaque ligne de la collection  <
            //  ( On récupère dans "ItemColumn" un dictionnaire qui correspond...
            //    ...à un enregistrement ) 
            foreach (var ItemColumn in _RowsDictColumnsValue)
            {
                // - - -  Pour l'illustration du code ==> utilise deux moyens différents - - - 
                //   1 - Création d'une nouvelle INSTANCE 
                // > Création d'une Instance Dynamique à partir du Type Builder < 
                object instanceModel = Activator.CreateInstance(_DynamicModel.CreateType());

                //   2 - Création d'un nouveau TYPE 
                Type type = _DynamicModel.CreateType();

                // > Initilaise Dictionnaire qui va recevoir la ligne <
                RowDictColumnsValue.Clear();

                // > Récupére la ligne de l'enregistrement <
                //   (  On va parcourir TOUTES les colonnes de l'enregistrement ) 
                RowDictColumnsValue = ItemColumn;

                foreach (var Column in RowDictColumnsValue)
                {
                
                    // > Construit une propriété à partie du nom de la colonne en cours <
                    PropertyInfo property = instanceModel.GetType().GetProperty(Column.Key);

                    //  On verifie que l'on peut ECRIRE <
                    if (property != null && property.CanWrite)
                    {
                        // > Voir : https://learn.microsoft.com/fr-fr/dotnet/api/system.reflection.emit.typebuilder.defineproperty?view=net-8.0 <

                        // > Voir https://helpcentral.componentone.com/nethelp/c1datagridwpf/C1.WPF.DataGrid.4~C1.WPF.DataGrid.CustomPropertyInfo~SetValue(Object,Object,BindingFlags,Binder,Object[],CultureInfo).html <
                        // > Voir https://stackoverflow.com/questions/15926338/correct-the-parameter-count-mismatch <
                        // > Voir https://learn.microsoft.com/fr-fr/dotnet/api/system.reflection.propertyinfo.setvalue?view=net-8.0 <

                        // > Voir https://stackoverflow.com/questions/50374060/reflection-propertyinfo-setvalue-c-sharp <
                        // > Voir https://stackoverflow.com/questions/31835823/parameter-count-mismatch-in-property-getvalue <
                                               
                        try
                        {
                            // > Ces deux manières fonctionnes <
                            //  1 - => "SetValue" sur l'instance "property" 
                            property.SetValue(instanceModel, Column.Value, null);

                            //  2 - => "InvokeMember" sur l'instance "type" qui est le résultat de "_DynamicModel.CreateType();" 
                            // > Voir : https://learn.microsoft.com/fr-fr/dotnet/api/system.reflection.emit.typebuilder.defineproperty?view=net-8.0 <
                            type.InvokeMember(Column.Key, BindingFlags.SetProperty,
                                     null, instanceModel, new object[] { Column.Value });
                        }
                        catch(Exception ex) 
                        {
                            MsgJobStatus = ex.ToString();
                            return MsgJobStatus;


                        }

                    }

                }

                // > Ajout de l'instance du modèle dynamiquue avec les ....
                //   ... valeurs mappées sur les bonnes colonnes <
                OutListInstanceMapped.Add(instanceModel);   

            }

            MsgJobStatus = string.Empty;
            return MsgJobStatus;


        }

        #endregion
    }
}
