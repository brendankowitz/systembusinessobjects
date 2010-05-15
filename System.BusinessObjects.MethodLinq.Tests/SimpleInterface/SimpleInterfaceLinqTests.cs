using System;
using System.Linq;
using System.BusinessObjects.MethodLinq.Tests.SimpleInterface.InterfaceContext;
using Moq;
using System.Diagnostics;
using System.BusinessObjects.MethodLinq.Tests.SimpleInterface.TestObjects;
using Xunit;

namespace System.BusinessObjects.MethodLinq.Tests.SimpleInterface
{
    public class SimpleInterfaceLinqTests
    {
         Mock<ISimpleInterface> mockService = null;

         public SimpleInterfaceLinqTests()
        {
            mockService = new Mock<ISimpleInterface>();
        }

        [Fact]
        public void CanQueryBasic()
        {
            var query = from rate in mockService.Object.Query().Rates
                          where
                          rate.Parameters.FromDate == DateTime.Today &&
                          rate.Building.ID == 501 &&
                          rate.Parameters.ToDate == DateTime.Today.AddDays(3)
                          select rate.Building;

            Trace.WriteLine(query.ToString());
            var response = query.ToList();
        }

        [Fact]
        public void CanQueryBasicLambda()
        {
            var query = mockService.Object.Query().Rates
                        .Where(r => r.Building.ID == 501)
                        .Where(r => r.Parameters.FromDate == DateTime.Today)
                        .Where(r => r.Parameters.ToDate == DateTime.Today.AddDays(3));

            Trace.WriteLine(query.ToString());
            var response = query.ToList();
        }

        [Fact]
        public void CanQueryBasicWithSelectLambda()
        {
            var query = from rate in mockService.Object.Query().Rates
                        where
                        rate.Parameters.FromDate == DateTime.Today &&
                        rate.Building.ID == 501 &&
                        rate.Parameters.ToDate == DateTime.Today.AddDays(3)
                        select new { Rates = rate.RetVal };

            Trace.WriteLine(query.ToString());
            var response = query.ToList();
        }

        [Fact]
        public void CanQueryBasicSetObject()
        {
            RequestParameters p = new RequestParameters
            {
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date.AddDays(3)
            };

            mockService.Setup(s => s.GetRates(It.Is<Building>(b => b.ID == 501),
               It.IsAny<Scope>(),
               It.Is<RequestParameters[]>(o => o[0].FromDate == p.FromDate && o[0].ToDate == p.ToDate)
               ))
               .Verifiable();

            var query = from rate in mockService.Object.Query().Rates
                        where
                        rate.Building.ID == 501 &&
                        rate.Parameters == p
                        select rate;

            Trace.WriteLine(query.ToString());
            var response = query.ToList();

            mockService.Verify();
        }

        [Fact]
        public void CanQueryBasicSetObjectThenEdit()
        {
            RequestParameters p = new RequestParameters
            {
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date.AddDays(3)
            };

            mockService.Setup(s => s.GetRates(It.Is<Building>(b => b.ID == 501),
               It.IsAny<Scope>(),
               It.Is<RequestParameters[]>(o => o[0].FromDate == p.FromDate && o[0].ToDate == p.ToDate && o[0].Package == "ABC")
               ))
               .Verifiable();

            var query = from rate in mockService.Object.Query().Rates
                        where
                        rate.Building.ID == 501 &&
                        rate.Parameters == p &&
                        rate.Parameters.Package == "ABC"
                        select rate;

            Trace.WriteLine(query.ToString());
            var response = query.ToList();

            mockService.Verify();
        }
    }
}
