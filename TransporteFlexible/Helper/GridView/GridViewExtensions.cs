using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace TransporteFlexible.Helper.GridView
{
    public abstract class GridViewExtensions<T> : System.Web.UI.Page
    {
        public DataTable ConvertToDataTable(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                if (!NoNecesaryFieldsInGridView(prop))
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

                }
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (!NoNecesaryFieldsInGridView(prop))
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }

        internal abstract void LoadDataGridView(List<T> entities);

        internal abstract bool NoNecesaryFieldsInGridView(PropertyDescriptor prop);
    }
}