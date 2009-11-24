using System;
using System.BusinessObjects.MethodLinq.Tests.SimpleInterface.TestObjects;

namespace System.BusinessObjects.MethodLinq.Tests.SimpleInterface.InterfaceContext
{
    [QueryableMethodAttribute(MethodName = "GetRates", Types = new Type[] { typeof(Building), typeof(Scope), typeof(RequestParameters[]) })]
    public class LinqBuildingRates : MethodQueryData<Rate>
    {
        /// <summary>
        /// Query Building
        /// </summary>
        [QueryableMethodParameterAttribute(ParameterName = "building")]
        public Building Building { get; set; }

        /// <summary>
        /// Query Parameters
        /// </summary>
        [QueryableMethodParameterAttribute(ParameterName = "parameterList")]
        public RequestParameters Parameters { get; set; }

        /// <summary>
        /// Query Scope
        /// </summary>
        [QueryableMethodParameterAttribute(ParameterName = "scope")]
        public Scope Scope { get; set; }
    }
}
