using System.Threading;
using System.Web;

namespace DataAccess.Concrete
{
    public class Common
    {
        public static bool ExistsObjectInContext(string labelObject)
        {
            return (HttpContext.Current.Items[labelObject] != null);
        }

        public static object GetObjectInContext(string labelObject)
        {
            return HttpContext.Current.Items[labelObject];
        }

        public static void AddObjectInContext(string labelObject, object pObjeto)
        {
            Mutex mMutex = new Mutex();

            mMutex.WaitOne();
            HttpContext.Current.Items.Remove(labelObject);
            HttpContext.Current.Items.Add(labelObject, pObjeto);
            mMutex.ReleaseMutex();
        }

        public static void RemoveObjectInContext(string labelObject)
        {
            Mutex mMutex = new Mutex();

            mMutex.WaitOne();
            if (HttpContext.Current.Items[labelObject] != null)
                HttpContext.Current.Items.Remove(labelObject);

            mMutex.ReleaseMutex();
        }
    }
}
