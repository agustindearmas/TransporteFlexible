using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace TransporteFlexible.Helper
{
    public class TablaHelper<T>
    {
        public TableRow[] BuildTable(List<T> datos)
        {
            TableRow[] _table = new TableRow[] { };            
            if (datos.Count > 0)
            {
                object dato = datos.First();
                _table.Append(BuildHeaders(dato.GetType()));
                //BuildBody(datos);               
            }
            return _table;
        }

        private TableHeaderRow BuildHeaders(Type type)
        {
            if (type != null)
            {
                TableHeaderRow thr = new TableHeaderRow();
                foreach (var prop in type.GetProperties())
                {
                    TableHeaderCell thc = new TableHeaderCell
                    {
                        Text = NombrePropiedadATitulo(prop.Name)
                    };
                    thr.Cells.Add(thc);
                }
                return thr;
            }
            return null;
        }

        private TableRow[] BuildBody(List<T> datos)
        {
            if (datos != null)
            {
                TableRow[] trc = new TableRow[] { };
                foreach (var data in datos)
                {
                    TableRow tr = new TableRow();
                    foreach (var prop in data.GetType().GetProperties())
                    {
                        TableCell tc = new TableCell
                        {
                            Text = Convert.ToString(prop.GetValue(data))
                        };
                        tr.Cells.Add(tc);
                    }
                    trc.Append(tr);
                }
                return trc;
            }
            return null;
        }

        private string NombrePropiedadATitulo(string nombreProp)
        {
            for (int i = 1; i < nombreProp.Length; i++)
            {
                char c = Convert.ToChar(nombreProp[i]);
                if (Char.IsUpper(c))
                {
                    nombreProp.Insert(i - 1, " ");
                }
            }
            return nombreProp;
        }
    }
}