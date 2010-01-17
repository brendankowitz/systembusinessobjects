using System;
using System.BusinessObjects.Kernel;
using Autofac;

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
