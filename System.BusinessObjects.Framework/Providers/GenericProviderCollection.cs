using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;

namespace System.BusinessObjects.Providers
{
    public class GenericProviderCollection<T> : ProviderCollection where T : System.Configuration.Provider.ProviderBase
    {
        public new T this[string name]
        {
            get { return (T)base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!(provider is T))
                throw new ArgumentException
                    ("Invalid provider type", "provider");

            base.Add(provider);
        }
    }
}
