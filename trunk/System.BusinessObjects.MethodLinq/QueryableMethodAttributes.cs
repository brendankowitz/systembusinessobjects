using System;

namespace System.BusinessObjects.MethodLinq
{
    /// <summary>
    /// Defines the method name and types used to call that method. This is the bases
    /// for the framework's ability to build parameters and invoke the correct method
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class QueryableMethodAttribute : Attribute
    {
        public string MethodName { get; set; }
        public Type[] Types { get; set; }
    }

    /// <summary>
    /// Defines a property as a parameter in the method. This is used to build this parameter
    /// type from the linq query.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class QueryableMethodParameterAttribute : Attribute
    {
        public string ParameterName { get; set; }
    }
}
