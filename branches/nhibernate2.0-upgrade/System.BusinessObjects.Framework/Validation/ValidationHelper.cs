using System.BusinessObjects.Data;
using System.Reflection;

namespace System.BusinessObjects.Validation
{
    internal class ValidationHelper
    {
        public static PropertyInfo getPropertyInfo(string name, DataObject obj)
        {
            return obj.GetType().GetProperty(name);
        }
    }
}
