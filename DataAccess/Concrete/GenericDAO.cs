using Common.Attributes;
using Common.Enums.Mappings;
using Common.Extensions;
using DataAccess.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace DataAccess.Concrete
{
    public class GenericDAO<T> where T : class, new()
    {
        public static IDataBase DataBase { get; set; }
        public static IMapper Mapper { get; set; }

        static GenericDAO()
        {
            var TableAtt = (TableAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));

            if (TableAtt == null) throw new Exception("La clase " + typeof(T).Name + " debe tener el atributo Table para resolver la comunicación con DB");

            if (TableAtt.Mapping == DAOMapperType.XML)
                Mapper = new Mapper();
            else
                Mapper = new EntityMapper();

            if (TableAtt.Motor == SQLType.SqlServer)
                DataBase = new DataBase();
            // else
            // DataBase = new DataBaseOracle();
        }

        /// <summary>
        /// Persiste una entidad en base de datos. Puede crear o actualizar la misma.
        /// </summary>
        /// <param name="entity">Cualquier entidad definida en el mapping.</param>
        /// <returns>Identificador de la entidad persistida.</returns>
        public static int Save(T entity)
        {
            var spName = GetSpName(IsUpdate(entity) ? "spNameUpdate" : "spNameInsert");

            int idEntity = 0;

            using (var scope = new TransactionScope())
            {
                idEntity = DataBase.ExecuteSPNonQuery(spName, (p) => FillParameters(p, entity));

                if (HasChilds())
                {
                    //Asigno a la clase el id generado
                    if (!IsUpdate(entity))
                        SetPropertyValue(Mapper.GetPropertyDBKey(entity.GetType())[0], idEntity, entity);

                    UpdateRelation(entity);
                }

                scope.Complete();
            }

            return idEntity;
        }

        private static void UpdateRelation(T entity)
        {
            foreach (var prop in GetPropertiesList(entity))
            {
                TableAttribute TableAtt = (TableAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));
                var childType = prop.PropertyType.GenericTypeArguments[0].GetTypeInfo();
                TableAttribute SonTableAtt = (TableAttribute)Attribute.GetCustomAttribute(childType, typeof(TableAttribute));
                string nameRelation = TableAtt.ProcedureName + SonTableAtt.ProcedureName;
                
                var idParent = GetPropertyValue(Mapper.GetPropertyDBKey(entity.GetType())[0], entity);
                //Considerando entidades de una sola clave

               

                DataBase.ExecuteSPNonQuery(GetSpName("spNameDelete", TableAtt.Schema.GetName(), nameRelation, null), p => FillParametersRelation(p, idParent));

                var lista = (IList)prop.GetValue(entity, null);

                if ((lista != null))
                {
                    foreach (object item in lista)
                    {
                        var idChild = GetPropertyValue(Mapper.GetPropertyDBKey(item.GetType())[0], item);
                        //Considerando entidades de una sola clave
                        DataBase.ExecuteSPNonQuery(GetSpName("spNameInsert", TableAtt.Schema.GetName(), nameRelation, null), p => FillParametersRelation(p, idParent, idChild));
                    }
                }
            }
        }

        private static bool HasChilds()
        {
            return typeof(T).GetProperties()
                   .Any(p => typeof(IList).IsAssignableFrom(p.PropertyType) && p.PropertyType.IsGenericType);
        }

        private static bool IsUpdate(T entity)
        {
            Type tipo = entity.GetType();
            string[] camposClave = Mapper.GetPropertyDBKey(tipo);
            string[] camposClaveFk = Mapper.GetPropertyDBKeyAndKeyChild(tipo);

            object valor = null;
            object valorFk = null;
            bool Return = true;
            if (camposClaveFk.Length > 0)
            {
                for (int s = 0; s < camposClaveFk.Length; s++)
                {
                    valor = GetPropertyValue(camposClave[s], entity);
                    if ((valor == null) || string.IsNullOrEmpty(valor.ToString()) || valor.ToString() == "0")
                    {
                        //for (int i = 0; i <= camposClaveFk.Length - 1; i++)
                        //{
                        valorFk = GetPropertyValue(camposClaveFk[s], entity);
                        if ((valorFk != null) && (string.IsNullOrEmpty(valorFk.ToString()) || valorFk.ToString() != "0"))
                        {
                            //for (int r = 0; r <= camposClave.Length - 1; r++)
                            //{
                            valor = GetPropertyValue(camposClave[s], entity);
                            if ((valor == null) || string.IsNullOrEmpty(valor.ToString()) || valor.ToString() == "0")
                            {
                                SetPropertyValue(camposClave[s], valorFk, entity);
                                Return = false;
                            }
                            //}

                        }
                        //}
                    }
                }
            }
            for (int l = 0; l <= camposClave.Length - 1; l++)
            {
                valor = GetPropertyValue(camposClave[l], entity);
                if ((valor == null) || string.IsNullOrEmpty(valor.ToString()) || valor.ToString() == "0")
                {
                    Return = false;
                }
            }

            return Return;
        }

        private static List<PropertyInfo> GetPropertiesList(object entity)
        {
            return entity.GetType().GetProperties()
                   .Where(p => typeof(IList).IsAssignableFrom(p.PropertyType) && p.PropertyType.IsGenericType).ToList();
        }

        private static void FillParameters(DbParameterCollection parametros, object entity)
        {
            if (entity == null) return;

            foreach (DbParameter parametro in parametros)
            {
                if (parametro.Direction == ParameterDirection.ReturnValue) continue;

                string prop = Mapper.GetPropertyName(parametro, entity.GetType());
                if (string.IsNullOrWhiteSpace(prop)) continue;

                object valor = GetPropertyValue(prop, entity);
                if ((valor == null) || string.IsNullOrWhiteSpace(valor.ToString()))
                {
                    parametro.Value = DBNull.Value;
                }
                else
                {
                    parametro.Value = valor;
                }
            }
        }

        private static void FillParametersRelation(DbParameterCollection parametros, object idParent, object idChild = null, object relationEntity = null)
        {
            if (idChild != null)
            {
                parametros[1].Value = idParent;
                parametros[2].Value = idChild;
            }
            else
            {
                parametros[1].Value = idParent;
            }
            //IMPLEMENTAR SI ES NECESARIO QUE UNA ENTIDAD DE RELACION ADEMAS CONTENGA ATRIBUTOS PROPIOS, 
            //EN ESE CASO HAY QUE AGREGAR LA RELACION EN EL MAPPING COMO ENTIDAD.
            //if (relationEntity != null)
            //{
            //    for (var i = 0; i <= parametros.Count - 1; i++)
            //    {
            //
            //    }
            //}
        }

        //Funcion recursiva para obtener el valor de una propiedad de un objeto. Si la propiedad es otro objeto, se vuelve 
        //a llamar a si misma para obtener la propiedad del objeto hijo
        private static object GetPropertyValue(string propiedad, object entity)
        {
            if (entity == null) return null;
            Type tipo = entity.GetType();
            string[] objeto = propiedad.Split('.');

            if (objeto.Length == 2)
            {
                return GetPropertyValue(objeto[1], tipo.GetProperty(objeto[0].ToString()).GetValue(entity));
            }
            else if (objeto.Length == 1)
            {
                return tipo.GetProperty(propiedad).GetValue(entity);
            }
            else
            {
                throw new Exception("Se quiso obtener el valor de una propiedad formada por mas de un objeto.");
            }
        }

        private static void SetPropertyValue(string propiedad, object valor, object entity)
        {

            Type tipo = entity.GetType();
            string[] objeto = propiedad.Split('.');

            switch (objeto.Length)
            {
                case 0:
                    throw new Exception("Se quiso establer el valor de una propiedad sin nombre");
                case 1:
                    if (!DBNull.Value.Equals(valor))
                    {
                        tipo.GetProperty(propiedad).SetValue(entity, valor);
                    }
                    return;
                case 2:
                    Type tipoProp = tipo.GetProperty(objeto[0]).PropertyType;

                    object instancia = tipo.GetProperty(objeto[0]).GetValue(entity);

                    if (instancia == null)
                    {
                        instancia = Activator.CreateInstance(tipoProp);
                    }

                    tipo.GetProperty(objeto[0]).SetValue(entity, instancia);

                    if (!DBNull.Value.Equals(valor))
                    {
                        tipoProp.GetProperty(objeto[1]).SetValue(instancia, valor);
                    }
                    return;
                default:
                    Type tipoProp2 = tipo.GetProperty(objeto[0]).PropertyType;

                    object instancia2 = tipo.GetProperty(objeto[0]).GetValue(entity);

                    if (instancia2 == null)
                    {
                        instancia2 = Activator.CreateInstance(tipoProp2);
                    }

                    tipo.GetProperty(objeto[0]).SetValue(entity, instancia2);

                    if (!DBNull.Value.Equals(valor))
                    {
                        string prop = objeto.Skip(1).Aggregate((x, y) => x + "." + y);
                        SetPropertyValue(prop, valor, instancia2);
                    }
                    return;
            }
        }

        public static void Delete(int id)
        {
            if (!typeof(T).GetProperties().Any(p => p.Name == "Id" && p.CanWrite))
            {
                throw new Exception("La entidad no tiene definida una propiedad con el nombre 'Id' o es de solo lectura.");
            }

            var entity = new T();
            typeof(T).GetProperty("Id").SetValue(entity, id);

            Delete(entity);
        }

        public static void Delete(T entity)
        {
            DataBase.ExecuteSPNonQuery(GetSpName("spNameDelete"), p => FillParameters(p, entity));
        }

        public static List<T> Find(T filterEntity, string executionName = null)
        {
            List<T> lista = null;

            DataBase.ExecuteSPWithResultSet(GetSpName("spNameSelect", executionName), r =>
            {
                lista = (List<T>)BuildList(typeof(T), r);

            }, p => FillParameters(p, filterEntity));

            return lista;
        }

        public static List<T> GetAll()
        {
            return Find((T)null);
        }

        private static void FillChilds(object entity)
        {
            //Determino por reflection si tiene propiedades de tipo List y genero una coleccion en base al tipo de la coleccion, 
            //busco el SP correspondiente a la relacion y genero las entidades de la coleccion tipada para luego asignarla a la propiedad.
            //Por ultimo, por cada entidad creada vuelva a llamar recursivamente a FillChilds hasta terminar con todos los nodos hijos de la 
            //entidad creada.

            foreach (var prop in GetPropertiesList(entity))
            {
                var TableAtt = (TableAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));
                var childType = prop.PropertyType.GenericTypeArguments[0].GetTypeInfo();
                var SonAtt = (TableAttribute)Attribute.GetCustomAttribute(childType, typeof(TableAttribute));
                var nameRelation = TableAtt.ProcedureName + SonAtt.ProcedureName;

                var idParent = GetPropertyValue(Mapper.GetPropertyDBKey(entity.GetType())[0], entity);
                //Considerando entidades de una sola clave
         

                IList lista = null;

                //Funciones Anonimas Lambdas
                DataBase.ExecuteSPWithResultSet(GetSpName("spNameSelect", TableAtt.Schema.GetName(), nameRelation, null), r =>
                {
                    lista = BuildList(childType, r);
                }, p => FillParametersRelation(p, idParent));

                if ((lista != null) && lista.Count > 0)
                {
                    SetPropertyValue(prop.Name, lista, entity);

                }

            }
        }

        private static IList BuildList(Type tipo, DbDataReader reader)
        {
            IList lista = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { tipo }));

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var instancia = Activator.CreateInstance(tipo);
                    Fill(instancia, reader);
                    FillChilds(instancia);
                    lista.Add(instancia);
                }
            }
            return lista;
        }

        private static void Fill(object entity, IDataReader reader)
        {
            for (var i = 0; i <= reader.FieldCount - 1; i++)
            {
                string propiedad = Mapper.GetPropertyName(reader.GetName(i), entity.GetType());
                if (!string.IsNullOrEmpty(propiedad))
                {
                    SetPropertyValue(propiedad, reader[i], entity);
                }
            }
        }

        public static void Execute(T entity, string executionName)
        {
            DataBase.ExecuteSPNonQuery(GetSpName("spNameExecute", executionName), p => FillParameters(p, entity));
        }

        public static void ExecuteQuery(string query, string dbName)
        {
            DataBase.ExecuteScriptNonQuery(query, dbName);
        }

        public static int ExecuteScalarQuery(string query, string dbName)
        {
            return DataBase.ExecuteScalarScript(query, dbName);
        }



        private static string GetSpName(string nameOperation, string executionName = null)
        {
            var TableAtt = (TableAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));

            var schemaName = TableAtt.Schema != Schema.None ? TableAtt.Schema.ToString() : "dbo";//TableAtt.PrefixSp;
            return GetSpName(nameOperation, schemaName, TableAtt.ProcedureName, executionName);
        }

        private static string GetSpName(string nameOperation, string schema, string entityName, string executionName)
        {
            if (string.IsNullOrEmpty(executionName))
            {
                return string.Concat(schema, ".", entityName, "_", ConfigurationManager.AppSettings[nameOperation]);
            }
            else
            {
                return string.Concat(schema, ".", entityName, "_", ConfigurationManager.AppSettings[nameOperation], "_", executionName);
            }
        }
    }
}
