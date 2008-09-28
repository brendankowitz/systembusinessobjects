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
        /// Compares that a property is greater then the expected.
        /// </summary>
        public static ValidatorTemplate Greater<R>(DataObject obj, string propertyName, R expected) where R : IComparable
        {
            PropertyInfo info = Property.GetPropertyInfo(propertyName, obj);
            ValidatorTemplate temp = delegate(out string propName, out string message) //PropertyInfo info, DataObject objObj, R expectedVal
            {
                propName = propertyName;
                bool retval;
                message = string.Empty;
                object curVal = info.GetValue(obj, null); 
                switch (expected.CompareTo(curVal))
                {
                    case 1: retval = true; break;
                    case 0:
                    case -1: retval = false; message = string.Format("{0} had a value of {1}, but expected a value of more then {2}", propertyName, curVal, expected); 
                        break;
                    default: retval = false; break;
                }

                return retval;
            };
            return temp;
        }

        /// <summary>
        /// Compares that a property is less then the expected value by using IComparable.
        /// </summary>
        public static ValidatorTemplate Less<R>(DataObject obj, string propertyName, R expected) where R : IComparable
        {
            PropertyInfo info = Property.GetPropertyInfo(propertyName, obj);
            ValidatorTemplate temp = delegate(out string propName, out string message)
            {
                propName = propertyName;
                bool retval;
                message = string.Empty;
                object curVal = info.GetValue(obj, null);
                switch (expected.CompareTo(curVal))
                {
                    case -1: retval = true; break;
                    case 0:
                    case 1: retval = false; message = string.Format("{0} had a value of {1}, but expected a value of less then {2}", propertyName, curVal, expected);
                        break;
                    default: retval = false; break;
                }

                return retval;
            };
            return temp;
        }

        /// <summary>
        /// Validates that the property is not empty
        /// </summary>
        public static ValidatorTemplate IsNotEmpty(DataObject obj, string propertyName)
        {
            PropertyInfo info = Property.GetPropertyInfo(propertyName, obj);
            ValidatorTemplate temp = delegate(out string propName, out string message)
            {
                propName = propertyName;
                bool retval;
                message = string.Empty;
                object curVal = info.GetValue(obj, null);

                if (info.PropertyType == typeof(string))
                {
                    if (string.IsNullOrEmpty(curVal as string))
                    {
                        retval = false;
                        message = string.Format("{0} has an empty value where a value was expected.", propertyName);
                    }
                    else
                        retval = true;
                }
                else if (curVal is ICollection)
                {
                    if (((ICollection)curVal).Count == 0)
                    {
                        retval = false;
                        message = string.Format("Collection {0} was empty where 1 or more items are required.", propertyName);
                    }
                    else
                        retval = true;
                }
                else if(curVal == null)
                {
                    retval = false;
                    message = string.Format("{0} has an empty value where a value was expected.", propertyName);
                }
                else
                {
                    throw new ApplicationException(string.Format("Unspecified validation type {0} on property {1}", info.PropertyType.Name, propertyName));
                }

                return retval;
            };
            return temp;
        }

        /// <summary>
        /// Validates that a property is not null
        /// </summary>
        public static ValidatorTemplate IsNotNull(DataObject obj, string propertyName)
        {
            PropertyInfo info = Property.GetPropertyInfo(propertyName, obj);
            ValidatorTemplate temp = delegate(out string propName, out string message)
            {
                propName = propertyName;
                bool retval;
                message = string.Empty;
                object curVal = info.GetValue(obj, null);

                if (curVal == null)
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

        /// <summary>
        /// Validates that a string has a certain length or a collection is of a certain size
        /// </summary>
        public static ValidatorTemplate LengthGreater(DataObject obj, string propertyName, int expected)
        {
            PropertyInfo info = Property.GetPropertyInfo(propertyName, obj);
            ValidatorTemplate temp = delegate(out string propName, out string message)
            {
                propName = propertyName;
                bool retval;
                message = string.Empty;
                object curVal = info.GetValue(obj, null);

                if (info.PropertyType == typeof(string))
                {
                    if (string.IsNullOrEmpty((string)curVal) || ((string)curVal).Length < expected)
                    {
                        retval = false;
                        message = string.Format("{0} has a length of {1} where a length of {2} was expected.", propertyName, curVal == null ? 0 : ((string)curVal).Length, expected);
                    }
                    else
                        retval = true;
                }
                else if (curVal is ICollection)
                {
                    if (((ICollection)curVal).Count < expected)
                    {
                        retval = false;
                        message = string.Format("Collection {0} has a length of {1} where a minimum length of {2} was expected.", propertyName, ((ICollection)curVal).Count, expected);
                    }
                    else
                        retval = true;
                }
                else
                {
                    throw new ApplicationException(string.Format("Unspecified validation type {0} on property {1}", info.PropertyType.Name, propertyName));
                }

                return retval;
            };
            return temp;
        }

        /// <summary>
        /// Validates that a string is less than a certain length or a collection is less than a certain size
        /// </summary>
        public static ValidatorTemplate LengthLess(DataObject obj, string propertyName, int expected)
        {
            PropertyInfo info = Property.GetPropertyInfo(propertyName, obj);
            ValidatorTemplate temp = delegate(out string propName, out string message)
            {
                propName = propertyName;
                bool retval = false;
                message = string.Empty;
                object curVal = info.GetValue(obj, null);

                if (info.PropertyType == typeof(string))
                {
                    if (string.IsNullOrEmpty((string)curVal) || ((string)curVal).Length > expected)
                    {
                        retval = false;
                        message = string.Format("{0} has a length of {1}, a length of {2} or less was expected.", propertyName, curVal == null ? 0 : ((string)curVal).Length, expected);
                    }
                    else
                        retval = true;
                }
                else if (info.PropertyType == typeof(ICollection))
                {
                    if (((ICollection)curVal).Count > expected)
                    {
                        retval = false;
                        message = string.Format("Collection {0} has a length of {1} where a maximum length of {2} was expected.", propertyName, ((ICollection)curVal).Count, expected);
                    }
                    else
                        retval = true;
                }
                else if(curVal == null)
                {
                    
                }
                else
                {
                    throw new ApplicationException(string.Format("Unspecified validation type {0} on property {1}", info.PropertyType.Name, propertyName));
                }

                return retval;
            };
            return temp;
        }

        /// <summary>
        /// Validates that a string or collection is a certain length
        /// </summary>
        public static ValidatorTemplate LengthFixed(DataObject obj, string propertyName, int expected)
        {
            PropertyInfo info = Property.GetPropertyInfo(propertyName, obj);
            ValidatorTemplate temp = delegate(out string propName, out string message)
            {
                propName = propertyName;
                bool retval = false;
                message = string.Empty;
                object curVal = info.GetValue(obj, null);

                if (info.PropertyType == typeof(string))
                {
                    if (string.IsNullOrEmpty((string)curVal) || ((string)curVal).Length != expected)
                    {
                        retval = false;
                        message = string.Format("{0} has a length of {1}, a fixed length of {2} was expected.", propertyName, curVal == null ? 0 : ((string)curVal).Length, expected);
                    }
                    else
                        retval = true;
                }
                else if (info.PropertyType == typeof(ICollection))
                {
                    if (((ICollection)curVal).Count != expected)
                    {
                        retval = false;
                        message = string.Format("Collection {0} has a length of {1} where a fixed length of {2} was expected.", propertyName, ((ICollection)curVal).Count, expected);
                    }
                    else
                        retval = true;
                }
                else if (curVal == null)
                {

                }
                else
                {
                    throw new ApplicationException(string.Format("Unspecified validation type {0} on property {1}", info.PropertyType.Name, propertyName));
                }

                return retval;
            };
            return temp;
        }

        /// <summary>
        /// Validates that a string contains only the specified characters
        /// </summary>
        public static ValidatorTemplate ValidCharacterRange(DataObject obj, string propertyName, string validCharaters, bool caseSensitive)
        {
            PropertyInfo info = Property.GetPropertyInfo(propertyName, obj);
            ValidatorTemplate temp = delegate(out string propName, out string message)
            {
                propName = propertyName;
                bool retval = true;
                message = string.Empty;
                object curVal = info.GetValue(obj, null);
                string casedValid = caseSensitive ? validCharaters : validCharaters.ToUpper();

                if (info.PropertyType == typeof(string))
                {
                    if (!string.IsNullOrEmpty((string)curVal))
                    {
                        string strCurVal = caseSensitive ? ((string)curVal) : ((string)curVal).ToUpper();
                        for (int i = 0; i < strCurVal.Length; i++)
                        {
                            if (!casedValid.Contains(strCurVal[i].ToString()))
                            { //fail
                                retval = false;
                                message = string.Format("{0} contains an invalid character '{1}' the only valid charaters are '{2}'", propertyName, ((string)curVal)[i], validCharaters);
                                break;
                            }
                        }
                    }
                    else
                    {
                        retval = true;
                    }
                }
                else
                {
                    throw new ApplicationException(string.Format("Unspecified validation type {0} on property {1}", info.PropertyType.Name, propertyName));
                }

                return retval;
            };
            return temp;
        }

        /// <summary>
        /// Performs a regex validation against a property
        /// </summary>
        public static ValidatorTemplate Regex(DataObject obj, string propertyName, Regex expression, string errorMessage)
        {
            PropertyInfo info = Property.GetPropertyInfo(propertyName, obj);
            ValidatorTemplate temp = delegate(out string propName, out string message) //PropertyInfo info, DataObject objObj, R expectedVal
            {
                propName = propertyName;
                bool retval;
                message = string.Empty;
                object curVal = info.GetValue(obj, null);
                retval = expression.IsMatch(curVal.ToString());
                if (!retval)
                    message = string.Format(errorMessage, propertyName);
                return retval;
            };
            return temp;
        }
    }
}
