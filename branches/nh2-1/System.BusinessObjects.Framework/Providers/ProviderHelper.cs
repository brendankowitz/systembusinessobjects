using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Web;

namespace System.BusinessObjects.Providers
{
    /// <summary>
    /// A helper class for loading providers from the configuration file
    /// </summary>
    public class ProviderHelper
    {
        static object syncObject = new object();
        static Dictionary<string,ProviderSectionHandler> providerSection = new Dictionary<string,ProviderSectionHandler>();

        public static object GetSection(string sectionName)
        {
            if (HttpContext.Current == null)
                return ConfigurationManager.GetSection(sectionName);
            else
                return WebConfigurationManager.GetSection(sectionName);
        }

        public static T LoadProvider<T>(string sectionName, string providerName) where T : ProviderBase
        {
            Debug.Assert(!string.IsNullOrEmpty(providerName));

            ProviderSectionHandler section;
            if (!providerSection.TryGetValue(sectionName, out section))
            {
                lock (syncObject)
                {
                    if (!providerSection.TryGetValue(sectionName, out section))
                    {
                        // Get a reference to the provider section
                        section = (ProviderSectionHandler)GetSection(sectionName);
                        providerSection.Add(sectionName, section);
                    }
                }
            }

            Debug.Assert(section != null);

            T provider = (T)ProvidersHelper.InstantiateProvider(section.Providers[providerName], typeof(T));

            if (provider == null)
                throw new ProviderException(string.Format("Unable to load default '{0}' provider", sectionName));

            return provider;
        }

        public static T LoadDefaultProvider<T>(string sectionName) where T : ProviderBase
        {
            // Get a reference to the provider section
            ProviderSectionHandler section;
            if (!providerSection.TryGetValue(sectionName, out section))
            {
                lock (syncObject)
                {
                    if (!providerSection.TryGetValue(sectionName, out section))
                    {
                        // Get a reference to the provider section
                        section = (ProviderSectionHandler)GetSection(sectionName);
                        providerSection.Add(sectionName, section);
                    }
                }
            }

            Debug.Assert(section != null);
            return LoadProvider<T>(sectionName, section.DefaultProvider);
        }

        public static GenericProviderCollection<T> LoadProviderCollection<T>(string sectionName) where T : ProviderBase
        {
            // Get a reference to the provider section
            ProviderSectionHandler section;
            if (!providerSection.TryGetValue(sectionName, out section))
            {
                lock (syncObject)
                {
                    if (!providerSection.TryGetValue(sectionName, out section))
                    {
                        // Get a reference to the provider section
                        section = (ProviderSectionHandler)GetSection(sectionName);
                        providerSection.Add(sectionName, section);
                    }
                }
            }

            Debug.Assert(section != null);

            // Load registered providers and point _provider
            // to the default provider
            GenericProviderCollection<T> providers = new GenericProviderCollection<T>();
            ProvidersHelper.InstantiateProviders(section.Providers, providers, typeof(T));

            return providers;
        }

        public static ProviderCollection LoadProviderCollection(string sectionName, Type type)
        {
            // Get a reference to the provider section
            ProviderSectionHandler section;
            if (!providerSection.TryGetValue(sectionName, out section))
            {
                lock (syncObject)
                {
                    if (!providerSection.TryGetValue(sectionName, out section))
                    {
                        // Get a reference to the provider section
                        section = (ProviderSectionHandler)GetSection(sectionName);
                        providerSection.Add(sectionName, section);
                    }
                }
            }

            Debug.Assert(section != null);

            // Load registered providers and point _provider
            // to the default provider
            ProviderCollection providers = new ProviderCollection();
            ProvidersHelper.InstantiateProviders(section.Providers, providers, type);

            return providers;
        }
    }

    public class ProviderSectionHandler : ConfigurationSection
    {
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }

        [ConfigurationProperty("defaultProvider")]
        public string DefaultProvider
        {
            get { return (string)base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }
    }
}
