using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaypalAPI;

namespace System.BusinessObjects.MethodLinq.Tests.PaypalSample.PaypalContext
{
    public class PaypalContext : MethodContext<PayPalAPIAAInterface>
    {
        public PaypalContext(PayPalAPIAAInterface webservice)
            : base(webservice)
        {
            
        }

        public MethodLinqQuery<LinqSetExpressCheckout> SetExpressCheckout { get { return new MethodLinqQuery<LinqSetExpressCheckout>(new MethodQueryProvider(base.OwnerClass)); } }
    }

    /// <summary>
    /// Provides extension methods for Paypal
    /// </summary>
    public static class PaypalContextExtensions
    {
        /// <summary>
        /// Provides a Context to perform Linq Queries
        /// </summary>
        public static PaypalContext Query(this PayPalAPIAAInterface service)
        {
            return new PaypalContext(service);
        }
    }
}
