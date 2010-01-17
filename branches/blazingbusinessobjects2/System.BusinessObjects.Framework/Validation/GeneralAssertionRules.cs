using System.BusinessObjects.Data;
using System.Collections;
using System.Reflection;
using System.BusinessObjects.Helpers;
using System.Text.RegularExpressions;

namespace System.BusinessObjects.Validation
{
    /// <summary>
    /// Brings validation to business objects by providing familiar assertion rules to those found in NUnit.
    /// </summary>
    public class GeneralAssertionTemplate
    {
        /// <summary>
        /// Validates that a property is not null by checking the DataObject.IsNull() function
        /// </summary>
        public static ValidatorTemplate IsBusinessObjectNotNull(DataObject obj, string propertyName)
        {
            PropertyInfo info = Property.GetPropertyInfo(propertyName, obj);
            ValidatorTemplate temp = delegate(out string propName, out string message)
            {
                propName = propertyName;
                bool retval;
                message = string.Empty;

                if (obj.IsNull(propertyName))
                {
                    retval = false;
                    message = string.Format("{0} is null where a value is expected.", propertyName);
                }
                else
                    retval = true;

                return retval;
            };
            return temp;
        }
    }
}
