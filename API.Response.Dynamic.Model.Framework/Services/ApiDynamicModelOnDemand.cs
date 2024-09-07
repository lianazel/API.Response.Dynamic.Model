using System.Reflection;
using System.Reflection.Emit;

namespace API.Responses.Dynamic.Model.Infrastructures.Services
{
    public class ApiDynamicModelOnDemand
    {
        #region methods 

        // > Constructeur <
        public ApiDynamicModelOnDemand() 
        { 
        
        }
        
                
        /// <summary>
        ///  Construction du corps de la classe dynamique ( ou "TypeBuilder" ) 
        /// </summary>
        /// <param name="_ClassName"> Nom de la classe à créer </param>
        /// <returns></returns>
        public TypeBuilder NewTypeBuilder(string _ClassName)
        {
            AssemblyName AssName = new AssemblyName("DynamicAssembly");

            // > Spécifie que l'assemblie dynamique doit être capable de s'exécuter. <
            // => Cela permet d'accéder aux types et membres crées dynamiquement dans l'assembly.
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(AssName,
           AssemblyBuilderAccess.Run);

            // > Pour résumer, méthode "DefineDynamicModule" permet de créer un module dynamique ...
            // ...dans l'assembly dynamique . On pouura y ajouter des : 
            // ==> des types,
            // ==> d'autres éléments dynamiques.
            ModuleBuilder Modbuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

            // > On définit ici une nouvelle classe dynamique <
            // ==> On passeen paramètre : 
            // >> Le Nom de la nouvelle classe,
            // >> Non niveau de protection,
            // >> La classe parente ( ici on a null)
            TypeBuilder typeBuild = Modbuilder.DefineType(_ClassName, TypeAttributes.Public, null);

            return typeBuild;
        }

        /// <summary>
        ///  > Ajout des propriétées dans la classe créée <
        /// </summary>
        /// <param name="_typeBuild">        La nouvelle classe créee dynamiquement  </param>
        /// <param name="_columnNamesList">  Liste des colonne à ajouter             </param>
        /// <param name="_DicTypeColumns">   Pour chaque colonne, le type            </param>
        /// <param name="_OutpropertiesInfos">  Renvoi la liste des propriétes dans le model dynamique </param>
        /// <returns> TypeBuilder </returns>
        public TypeBuilder AddDynamicProperty(TypeBuilder _typeBuild, List<string> _columnNamesList,
            Dictionary<string, Type> _DicTypeColumns, out List<PropertyInfo> _OutpropertiesInfos)
        {

            // > Voir : https://learn.microsoft.com/fr-fr/dotnet/api/system.reflection.emit.typebuilder.defineproperty?view=net-8.0 <

            // > Initialisation de la liste <
            _OutpropertiesInfos = new List<PropertyInfo>();

            // > On parcourt la liste des colonne à ajouter dans la nouvelle classe  <
            foreach (string columnName in _columnNamesList)
            {
                // - - - - - - - - - - - 
                // > Définir le champ sous-jacent correspondant à la proprité <
                // - - - - - - - - - - - 
                // Attention : ici, au lieu d'utiliser le "typeof" : 
                // "FieldBuilder fieldBuilder = _typeBuild.DefineField("_" + columnName.ToLower(), typeof(string), FieldAttributes.Public);"
                // ==> on met directement le type renvoyé par le dictionnaire 
                FieldBuilder fieldBuilder = _typeBuild.DefineField("_" + columnName.ToUpper(),
                    (_DicTypeColumns[columnName]), FieldAttributes.Public);


                // - - - - - - - - - -
                // > Définir la propritété correspondante  <
                // - - - - - - - - - -
                PropertyBuilder propertyBuilder = _typeBuild.DefineProperty(columnName,
                    PropertyAttributes.HasDefault, _DicTypeColumns[columnName], null);

                // - - - - - - - -
                // > Ajout du "propertyBuilder" crée dans la liste "_OutpropertiesInfos"  <
                // - - - - - - - -
                _OutpropertiesInfos.Add(propertyBuilder);

                // Microsfot - The property set and property get methods require a special
                // set of attributes.
                MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

                // - - - - - - - - - - -
                // > Définir l'accesseur "Getter" de la propriété   <
                // - - - - - - - - - - -
                MethodBuilder getterMethod = _typeBuild.DefineMethod("get_" + columnName,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, _DicTypeColumns[columnName], Type.EmptyTypes);

                ILGenerator getterIL = getterMethod.GetILGenerator();
                getterIL.Emit(OpCodes.Ldarg_0);
                getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getterIL.Emit(OpCodes.Ret);


                // - - - - - - - - - - -
                // > Définir l'accesseur "Setter" de la propriété   <
                // - - - - - - - - - - -
                // MethodBuilder setterMethod = _typeBuild.DefineMethod("set_" + columnName,
                //    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, _DicTypeColumns[columnName], Type.EmptyTypes);                                         

                // Remarque : le "getSetAttr" remplace ici le...
                // ..."MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig," de la syntaxe de la ligne en commentaire ci dessus 

                MethodBuilder setterMethod =
               _typeBuild.DefineMethod("set_" + columnName,
                                       getSetAttr,
                                       null,
                                       new Type[] { _DicTypeColumns[columnName] });

                ILGenerator setterIL = setterMethod.GetILGenerator();
                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Stfld, fieldBuilder);
                setterIL.Emit(OpCodes.Ret);

                // - - - - - - - - - - -
                // > Associer l'accesseur "Getter" à la propriété   <
                // - - - - - - - - - - -
                propertyBuilder.SetGetMethod(getterMethod);

                // - - - - - - - - - - -
                // > Associer l'accesseur "Setter" à la propriété   <
                // - - - - - - - - - - -
                propertyBuilder.SetSetMethod(setterMethod);

            }

            return _typeBuild;

        }

        #endregion

    }
}
