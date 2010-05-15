using System;
using System.Linq;

namespace System.BusinessObjects.Infrastructure
{
    public interface IFactory<T>
    {
        T Resolve();
    }

    public class DelegateFactory<T> : IFactory<T>
    {
        private Func<T> _resolve;
        public DelegateFactory(Func<T> resolve)
        {
            _resolve = resolve;
        }

        public T Resolve()
        {
            return _resolve();
        }
    }
}
