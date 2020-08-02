using Common.Attributes;
using Common.Enums.Seguridad;
using Negocio.Managers.Seguridad;
using System;
using System.Collections.Generic;

namespace Negocio.CheckDigit
{
    public abstract class CheckDigit<T>
    {
        protected void ValidateIntegrity(List<T> entities)
        {
            try
            {
                int dvv = 0;
                BDManager _bdMgr = new BDManager();
                foreach (T entity in entities)
                {
                    int dvhCalculado = AlgoritmoDVH(entity);
                    int dvh = Convert.ToInt32(entity.GetType().GetProperty("DVH").GetValue(entity));

                    if (dvhCalculado == dvh)
                    {
                        dvv += dvh;
                        continue;
                    }
                    else
                    {
                        LogManager _bitacoraMgr = new LogManager();
                        
                        var tableAtt = (TableAttribute)Attribute.GetCustomAttribute(entity.GetType(), typeof(TableAttribute));
                        string nombreTabla = string.Concat(tableAtt.Schema, '.', tableAtt.ProcedureName);
                        _bitacoraMgr.Create(LogCriticality.Alta, "Problema Integridad", "Error de integridad en la tabla " + nombreTabla + " en el registro con Id: " +
                            entity.GetType().GetProperty("Id").GetValue(entity).ToString(), 1); // 1 User sistema
                        _bdMgr.BloquearBase();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        public int CalculateRegistryIntegrity(T entidad)
        {   
            int dvh = AlgoritmoDVH(entidad);
            var tableAtt = (TableAttribute)Attribute.GetCustomAttribute(entidad.GetType(), typeof(TableAttribute));
            string nombreTabla = string.Concat(tableAtt.Schema, '.', tableAtt.ProcedureName);
            TablaDVVManager _tablaDVVMgr = new TablaDVVManager();

            Type typeEntidad = entidad.GetType();
            int entityId = Convert.ToInt32(typeEntidad.GetProperty("Id").GetValue(entidad));

            var att = (NameEntityAttribute)Attribute.GetCustomAttribute((entidad.GetType().GetProperty("Id")), typeof(NameEntityAttribute));
            string nombreId = att.IdEntity;

            _tablaDVVMgr.ActualizarDVV(nombreTabla, dvh, entityId, nombreId);
            return dvh;
        }

        public int ObtenerIntegridadRegistro(T entidad)
        {
            return AlgoritmoDVH(entidad);
        }

        #region Privada
        private int AlgoritmoDVH(T entity)
        {
            try
            {
                string cadena = ConcatenarPropiedadesDelObjeto(entity);
                int suma = SumarCaracteres(cadena);
                int contador = 0;
                string contadorAuxiliar;
                do
                {
                    contadorAuxiliar = contador.ToString();
                    suma += contador;
                    contador++;

                } while (EsMultiploDe10(suma));

                string dvh = string.Concat(suma.ToString(), contadorAuxiliar);
                return Convert.ToInt32(dvh);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private int SumarCaracteres(string cadena)
        {
            try
            {
                int acumuladorPares = 0;
                int acumuladorImpares = 0;
                for (int i = 0; i < cadena.Length; i++)
                {
                    if (EsPar(i))
                    {
                        acumuladorPares += cadena[i];
                    }
                    else
                    {
                        acumuladorImpares += cadena[i];
                    }
                }
                int resultadoMultiplicacion = acumuladorPares * 3;
                int suma = resultadoMultiplicacion + acumuladorImpares;
                return suma;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool EsPar(int numero)
        {
            return (numero % 2) == 0;
        }

        private bool EsMultiploDe10(int numero)
        {
            return (numero % 10) == 0;
        }

        public void AplicarIntegridadRegistros()
        {

        }
        #endregion

        #region Abstracta
        /// <summary>
        /// Valida que todos los registros de la tabla que corresponde tengan integridad y consistencia.
        /// </summary>
        public abstract void ValidarIntegridadRegistros();

        /// <summary> 
        /// Concatena todas las propiedades del objeto necesarias para el caculo de DVH
        /// </summary>
        protected abstract string ConcatenarPropiedadesDelObjeto(T entity);

        /// <summary>
        /// Aplica integridad en un registro especifico. Calcula el DVH
        /// </summary>
        /// <param name="entity">Es el registro a aplicarle integridad</param>
        protected abstract void AplicarIntegridadRegistro(T entity);

        /// <summary>
        /// Recalcula la integridad de todos los registros de la tabla que corresponde
        /// </summary>
        /// <returns>Retorna la suma de todos los DVH es decir el DVV</returns>
        public abstract void RecalcularIntegridadRegistros();
        #endregion
    }
}
