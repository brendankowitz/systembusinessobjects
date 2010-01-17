using System;
using System.Linq;

namespace System.BusinessObjects.Kernel
{
    public interface IFactory<T>
    {
        T Resolve();
    }
}
