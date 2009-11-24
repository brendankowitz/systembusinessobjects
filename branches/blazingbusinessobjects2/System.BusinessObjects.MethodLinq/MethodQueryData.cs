using System;

namespace System.BusinessObjects.MethodLinq
{
    /// <summary>
    /// Base class for defining queryable parameters on a method, as well as the singular result object type
    /// </summary>
    /// <typeparam name="TReturnType">Singular result object type</typeparam>
    public abstract class MethodQueryData<TReturnType>
    {
        /// <summary>
        /// Specifies the object the method query will return
        /// </summary>
        public TReturnType RetVal { get; set; }

        /// <summary>
        /// implicit cast from query data object to result format
        /// </summary>
        public static implicit operator TReturnType(MethodQueryData<TReturnType> obj)
        {
            return obj.RetVal;
        }
    }
}
