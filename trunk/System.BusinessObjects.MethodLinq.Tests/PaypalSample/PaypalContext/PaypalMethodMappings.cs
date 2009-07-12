using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaypalAPI;

namespace System.BusinessObjects.MethodLinq.Tests.PaypalSample.PaypalContext
{
    [QueryableMethodAttribute(MethodName = "SetExpressCheckout", Types = new Type[] { typeof(SetExpressCheckoutRequest) })]
    public class LinqSetExpressCheckout : MethodQueryData<SetExpressCheckoutResponse>
    {
        /// <summary>
        /// Queryable Parameter Request
        /// </summary>
        [QueryableMethodParameterAttribute(ParameterName = "request")]
        public SetExpressCheckoutRequest Request { get; set; }
    }
}
