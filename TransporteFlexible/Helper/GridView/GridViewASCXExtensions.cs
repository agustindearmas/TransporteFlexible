using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;

namespace TransporteFlexible.Helper.GridView
{
    public abstract class GridViewASCXExtensions<T> : System.Web.UI.UserControl
    {
        public DataTable ConvertToDataTable(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                if (!EsCampoNoNecesario(prop))
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

                }
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (!EsCampoNoNecesario(prop))
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }
        /// <summary>
        /// Este metodo llena el DataGrid View,
        /// </summary>
        /// <param name="entities">Son las entidades que poblara el data grid view</param>
        /// <param name="scope">Es una bandera que indica que al finalizar la carga del 
        /// data gridview el scope de la pagina sea el elemento recien creado</param>
        internal abstract void LoadDataGridView(List<T> entities);

        internal abstract bool EsCampoNoNecesario(PropertyDescriptor prop);
    }
}