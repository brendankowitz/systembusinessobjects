using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.BusinessObjects.MethodLinq.Tests.PaypalSample.PaypalContext;
using System.Diagnostics;

namespace System.BusinessObjects.MethodLinq.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class SimpleLinqTests
    {
        Mock<PaypalAPI.PayPalAPIAAInterface> mockService = null;

        public SimpleLinqTests()
        {
            mockService = new Mock<PaypalAPI.PayPalAPIAAInterface>();
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
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
