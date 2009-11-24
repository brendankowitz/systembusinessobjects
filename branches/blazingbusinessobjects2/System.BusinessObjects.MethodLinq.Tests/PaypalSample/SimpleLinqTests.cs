using System;
using System.Linq;
using Moq;
using System.BusinessObjects.MethodLinq.Tests.PaypalSample.PaypalContext;
using System.Diagnostics;
using Xunit;

namespace System.BusinessObjects.MethodLinq.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    public class PaypalSimpleLinqTests
    {
        Mock<PaypalAPI.PayPalAPIAAInterface> mockService = null;

        public PaypalSimpleLinqTests()
        {
            mockService = new Mock<PaypalAPI.PayPalAPIAAInterface>();
        }

        [Fact]
        public void CanQueryBasicReferenceTypeLambda()
        {
            mockService.Setup<PaypalAPI.SetExpressCheckoutResponse>(s => s.SetExpressCheckout(It.IsAny<PaypalAPI.SetExpressCheckoutRequest>()))
                .Returns(new PaypalAPI.SetExpressCheckoutResponse());

            PaypalAPI.PayPalAPIAAInterface service = mockService.Object;

            var query = from item in service.Query().SetExpressCheckout
                        where
                        item.Request.RequesterCredentials.Credentials.Username == "myaccount" &&
                        item.Request.RequesterCredentials.Credentials.Password == "mypassword" &&
                        item.Request.RequesterCredentials.Credentials.Signature == "signature" &&
                        item.Request.SetExpressCheckoutReq.SetExpressCheckoutRequest.SetExpressCheckoutRequestDetails.CallbackURL == "http://www.blah.com"
                        select new { Response = item.RetVal.SetExpressCheckoutResponse1 };

            Trace.WriteLine(query.ToString());
            var response = query.Single();
        }
    }
}
