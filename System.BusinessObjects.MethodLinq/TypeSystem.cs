using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System.BusinessObjects.MethodLinq
{
    /// <summary>
    /// A utility class for looking up nested element types
    /// </summary>
    /// <remarks>
    /// Based on code from: http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
    /// </remarks>
    internal static class TypeSystem
    {

        internal static Type GetPropertyType(MemberInfo member)
        {
            Type retval = null;
            if (member.MemberType == System.Reflection.MemberTypes.Property)
                retval = ((PropertyInfo)member).PropertyType;
            else if (member.MemberType == System.Reflection.MemberTypes.Field)
                retval = ((FieldInfo)member).FieldType;
            else
                throw new NotSupportedException(string.Format("Unable find System.Type of {0} of type {1}", member.Name, member.MemberType));
            return retval;
        }

        internal static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);

            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }

        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }
    }
}
