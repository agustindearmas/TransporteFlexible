using System.Collections.Generic;

namespace TransporteFlexible.Helper
{
    public static class SecurityHelper
    {
        public static bool CheckPermissions(int aValidar, object permisosSession)
        {
            List<int> permisos = (List<int>)permisosSession;
            if (permisos.Count <= 0)
            {
                return false;
            }
            else
            {
                return permisos.Contains(aValidar);
            }

        }
    }
}