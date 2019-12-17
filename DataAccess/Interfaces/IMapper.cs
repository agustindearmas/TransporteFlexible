using System;
using System.Data.Common;

namespace DataAccess.Interfaces
{
    public interface IMapper
    {
        string GetPropertyName(DbParameter parametro, Type TypeEntidad);

        string GetPropertyName(string campo, Type TypeEntidad);

        string GetAttributeDBName(string propiedad, Type TypeEntidad);

        string[] GetPropertyDBKey(Type TypeEntidad);

        //string GetEntityTable(Type TypeEntidad);

        string[] GetPropertyDBKeyAndKeyChild(Type TypeEntidad);
    }
}
