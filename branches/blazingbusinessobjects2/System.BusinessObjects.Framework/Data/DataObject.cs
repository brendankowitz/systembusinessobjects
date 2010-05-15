using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Collections;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using System.BusinessObjects.Validation;
using System.BusinessObjects.Helpers;

using NHibernate.Impl;
using NHibernate;
using NHibernate.Engine;
using Iesi.Collections;

#if DOT_NET_35
using System.Linq;
#endif

namespace System.BusinessObjects.Data
{
    /// <summary>
    /// An abstract class that provides core business object functionality.
    /// </summary>
    /// <remarks>
    /// Core functionality targets specifically:
    /// 1. tracking/cloning an object's persistence state
    /// 2. providing basic persistence functionality
    /// 3. implementing the built-in binding interfaces for use with .net's built-in datacontrols (ie. GridView)
    /// </remarks>
    [Serializable]
    public abstract class DataObject :  IDataErrorInfo, 
        INotifyPropertyChanged, INotifyPropertyChanging, NHibernate.Classic.IValidatable
    {
        #region Events
        public virtual event PropertyChangingEventHandler PropertyChanging;
        public virtual event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Protected Variables
        protected IDictionary<string, object> dataValue = new Dictionary<string, object>();
        private bool isDirty = false;
        private bool isDeleted = false;
        [NonSerialized, XmlIgnore]
        protected IValidationRuleCollection validationRules;

        /// <summary>
        /// Marks that the object has been modified
        /// </summary>
        public virtual bool IsDirty
        {
            protected internal set { isDirty = false; }
            get { return isDirty; }
        }

        /// <summary>
        /// Marks that the object is in a pending delete state
        /// </summary>
        public virtual bool IsDeleted
        {
            get { return isDeleted; }
        }
        #endregion

        #region .ctor
        /// <summary>
        /// Initializes a new instance of DataObject
        /// </summary>
        public DataObject()
        {
            initialiseValidationCollection();
        }

        private void initialiseValidationCollection()
        {
            Type ruleCollection = ConfigSectionHelper.GetValidationCollectionType();
            validationRules = Activator.CreateInstance(ruleCollection, this) as IValidationRuleCollection;
        }
        #endregion

        #region Get / Set Properties

        /// <summary>
        /// Gets a value from the internal data store, using the method name from the parent property
        /// If using this method remember to add: [MethodImpl( MethodImplOptions.NoInlining )] 
        /// to the method.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        protected T GetValue<T>()
        {
            string propertyName = new Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("PropertyName", "GetValue() failed because the property name could not be resolved.");
            }
            return GetValue<T>(propertyName.Substring(4));
        }

        /// <summary>
        /// Gets a value from the internal data store
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="keyName">Key of the object to fetch</param>
        /// <returns>the object's value</returns>
        protected T GetValue<T>(string keyName)
        {
            object obj = null;
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                obj = GetValue<object>(keyName, null);
            }
            else
            {
                obj = GetValue<object>(keyName, default(T));
            }
            return (T) obj;
        }

        /// <summary>
        /// Gets a value from the internal data store
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="keyName">Key of the object to fetch</param>
        /// <param name="defaultValue">A default value to return of the object's value doesn't exist</param>
        /// <returns>the object's value</returns>
        protected T GetValue<T>(string keyName, T defaultValue)
        {
            object obj;
            if (!dataValue.TryGetValue(keyName, out obj))
            {
                obj = defaultValue;
            }

            return (T)obj;
        }

        /// <summary>
        /// Sets a property value in the internal property store.
        /// If a null is passed the property will be reset and removed.
        /// Uses the method name from the parent property
        /// If using this method remember to add: [MethodImpl( MethodImplOptions.NoInlining )] 
        /// to the method.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        protected void SetValue(object value)
        {
            string propertyName = new Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException();
            }
            SetValue(propertyName.Substring(4), value);
        }

        /// <summary>
        /// Sets a property value in the internal property store.
        /// If a null is passed the property will be reset and removed.
        /// </summary>
        /// <param name="keyName">Property name of whos value is to be set</param>
        /// <param name="value">Value of the property</param>
        protected void SetValue(string keyName, object value)
        {
            //Fire property changing event
            string propertyName = keyName;
            if (PropertyChanging != null && !string.IsNullOrEmpty(propertyName))
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }

            //Update property value
            if (value == null) //set the property to null, this will work with the IsNull() method.
            {
                if (dataValue.ContainsKey(keyName)) { dataValue.Remove(keyName); }
            }
            else if (value is string)
            {
                dataValue[keyName] = ((string)value).Trim();
            }
            else
            {
                dataValue[keyName] = value;
            }

            //Fire property changed event
            if (PropertyChanged != null && !string.IsNullOrEmpty(propertyName))
            {
               OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Returns true if the property is null or has never been set
        /// </summary>
        public virtual bool IsNull(string propertyName)
        {
            bool isNull = !dataValue.ContainsKey(propertyName);
            if(!isNull)
            {
                object innerVal = GetValue<object>(propertyName, null);
                if (innerVal == null)
                {
                    isNull = true;
                } 
            }
            return isNull;
        }

        #endregion

        #region BeginEdit() / AcceptChanges() / RejectChanges()

        /// <summary>
        /// Calls other methods before the dataobject is edited
        /// </summary>
        protected void BeginEdit()
        {
            if (IsDeleted)
                throw new ApplicationException("Object has been marked for deletion and can not be modified.");
            isDirty = true;
        }

        #endregion

        /// <summary>
        /// Sets the state of this business object to deleted. Call Save() to update the datastore
        /// </summary>
        public virtual void MarkDeleted()
        {
            isDeleted = true;
        }

        /// <summary>
        /// Sets the business object's rowstate to indicate the object has been saved
        /// </summary>
        /// <returns></returns>
        protected virtual internal void SetSaveRowState()
        {
            isDirty = false;
        }

        #region Validation()
        /// <summary>
        /// A collection of validation rules used to determine if the data in the object is valid
        /// </summary>
        /// <remarks>
        /// Use 'set' to set your own instance/implementation of IValidationRuleCollection then override AddValidationRules to add your own rules.
        /// </remarks>
        [XmlIgnore]
        public virtual IValidationRuleCollection ValidationRules
        {
            get
            {
                if (validationRules == null)
                    initialiseValidationCollection();
                if (validationRules.Count == 0)
                {
                    AddValidationRules();
                    //automatically add rules
                    foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this))
                    {
                        validationRules.AddRulesFromAttributes(prop, prop.Attributes);
                    }
                }
                return validationRules;
            }
            set
            {
                validationRules = value;
            }
        }

        /// <summary>
        /// Override this function to add validation rules to this data object
        /// </summary>
        protected virtual void AddValidationRules()
        {
        }

        #region IDataErrorInfo Members

        /// <summary>
        /// Returns the first error in the object
        /// </summary>
        string IDataErrorInfo.Error
        {
            get
            {
                if (!ValidationRules.Validate() && ValidationRules.Messages.Length > 0)
                    return ValidationRules.Error;
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the error for a specific column name
        /// </summary>
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                return ((IDataErrorInfo)ValidationRules)[columnName];
            }
        }

        void NHibernate.Classic.IValidatable.Validate()
        {
            if (ValidationRules.Count > 0 && !IsDeleted)
            {
                if (!ValidationRules.Validate())
                    throw new NHibernate.Classic.ValidationFailure(ValidationRules.Error);
            }
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged Members

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                try
                {
                    PropertyChanged(this, e);
                }
                catch (NullReferenceException)
                {
                    PropertyChanged(this, e);
                }
            }
        }

        #endregion
    }
}
