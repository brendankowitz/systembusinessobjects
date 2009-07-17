using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.BusinessObjects.MethodLinq.Tests.SimpleInterface.InterfaceContext
{
    public class SimpleInterfaceContext : MethodContext<ISimpleInterface>
    {
        public SimpleInterfaceContext(ISimpleInterface simpleInterface)
            : base(simpleInterface)
        {

        }

        public MethodLinqQuery<LinqBuildingRates> Rates { get { return new MethodLinqQuery<LinqBuildingRates>(new MethodQueryProvider(base.OwnerClass)); } }
    }

    /// <summary>
    /// Provides extension methods for SimpleInterface
    /// </summary>
    public static class SimpleInterfaceExtensions
    {
        /// <summary>
        /// Provides a Context to perform Linq Queries
        /// </summary>
        public static SimpleInterfaceContext Query(this ISimpleInterface simpleInterface)
        {
            return new SimpleInterfaceContext(simpleInterface);
        }
    }
}
