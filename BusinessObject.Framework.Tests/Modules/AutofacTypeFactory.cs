using System;
using Autofac;
using System.BusinessObjects.Infrastructure;

namespace BusinessObject.Framework.Tests.Modules
{
    public class AutofacTypeFactory<T> : IFactory<T>
    {
        Container _container;

        public AutofacTypeFactory(Container container)
        {
            _container = container;
        }

        public T Resolve()
        {
            return _container.Resolve<T>();
        }

    }
}
