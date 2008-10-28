using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.BusinessObjects.Validation;
using System.BusinessObjects.Providers;
using System.Configuration;

namespace System.BusinessObjects.Helpers
{
    public static class ConfigSectionHelper
    {
        static NameValueConfigurationCollection settings = null;
        static readonly object settingsLock = new object();

        private static NameValueConfigurationCollection getSettings()
        {
            if (settings == null)
            {
                lock (settingsLock)
                {
                    settings = ProviderHelper.GetSection("DataObject.Settings") as NameValueConfigurationCollection;
                }
            }
            return settings;
        }

        static Type _validationCollection = null;
        public static Type GetValidationCollectionType()
        {
            if (_validationCollection == null)
            {
                if (getSettings() != null)
                foreach (System.Configuration.NameValueConfigurationElement element in getSettings())
                {
                    if (element.Name == "IValidationRuleCollection")
                    {
                        _validationCollection = Type.GetType(element.Value);
                        break;
                    }
                }
                if (_validationCollection == null) //use the default validation
                    _validationCollection = typeof(ValidationRuleCollection);
            }
            return _validationCollection;
        }
    }
}
