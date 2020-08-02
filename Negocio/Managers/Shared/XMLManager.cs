using Common.Enums.Seguridad;
using Common.FactoryMensaje;
using Common.Satellite.Shared;
using Negocio.Managers.Seguridad;
using System;
using System.Data;
using System.Xml.Linq;

namespace Negocio.Managers.Shared
{
    public class XMLManager
    {
        private readonly LogManager _logMgr;
        public XMLManager()
        {
            _logMgr = new LogManager();
        }
        public Message ExportDataTableToXMLFile(DataTable dt, string tableName, string filename)
        {
            try
            {
                string path = @"C:\Users\ajfde\Desktop\" + filename;
                
                dt.TableName = tableName;
                dt.WriteXml(path, true);
                XDocument file = XDocument.Load(path);
                file.Root.Name = tableName + "s";
                file.Save(path);
                return MessageFactory.GetMessage("MS79");
            }
            catch (Exception e)
            {
                try
                {
                    _logMgr.Create(LogCriticality.Alta, "ExportDataTableToXMLFile", "Se produjo una excepción exportando un Data Table a .xml" + e.Message, 1); // 1 User sistema
                }
                catch { }
                return MessageFactory.GettErrorMessage("ER03", e);
            }
        }
    }
}
