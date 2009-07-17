using System;
using System.BusinessObjects.MethodLinq.Tests.SimpleInterface.TestObjects;
using System.Collections.Generic;

namespace System.BusinessObjects.MethodLinq.Tests.SimpleInterface.InterfaceContext
{
    public interface ISimpleInterface
    {
        List<Rate> GetRates(Building building, Scope scope, params RequestParameters[] parameterList);
    }
}
