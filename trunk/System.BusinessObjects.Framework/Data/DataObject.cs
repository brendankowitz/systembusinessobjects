using System.BusinessObjects.Validation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.BusinessObjects.Providers;

namespace System.BusinessObjects.Data
{
    /// <summary>
    /// An abstract class that provides core business object funcationality.
    /// <remarks>
    /// Core functionality targets specifically:
    /// 1. tracking/cloning an object's persistance state
    /// 2. providing basic persistance functionality
    /// 3. implementing the built-in binding interfaces for use with .net's built-in datacontrols (ie. GridView)
    /// </remarks>
    /// </summary>
    [Serializable]
    public abstract class DataObject : ICloneable, IEditableObject, IDataErrorInfo, INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual event EventHandler OnDeleting;
        public virtual event EventHandler OnDeleted;
        public virtual event EventHandler OnSaving;
        public virtual event EventHandler OnSaved;
        #endregion

        #region Protected Variables
        protected IDictionary<string, object> foreignKeys = new Dictionary<string, object>();
        protected IDictionary<string, object> dataValue = new Dictionary<string, object>();
        protected IDictionary<string, object> oldDataValue = null;
        protected IDictionary<string, object> oldForeignKeys = null;
        protected DataRowState _rowstateOriginal;
        protected DataRowState _rowstate = DataRowState.Detached;
        [NonSerialized]
        protected ValidationRuleCollection validationRules = new ValidationRuleCollection();
        #endregion

        #region Public Properties
        /// <summary>
        /// The current rowstate of this business object
        /// </summary>
        public virtual DataRowState RowState
        {
            set { _rowstate = value; }
            get { return _rowstate; }
        }
        #endregion

        #region Enum QueryAction
        /// <summary>
        /// The type of query needed to persist this business object
        /// </summary>
        public enum QueryAction
        {
            Insert,
            Update,
            Delete,
            None
        }
        #endregion

        #region Get / Set Properties

        protected T GetValue<T>(string keyName)
        {
            object obj;

            if (typeof(T) == typeof(int))
            {
                obj = GetValue(keyName, -1);
            }
            else if (typeof(T) == typeof(bool))
            {
                obj = GetValue(keyName, false);
            }
            else if (typeof(T) == typeof(DateTime))
            {
                obj = GetValue(keyName, DateTime.MinValue);
            }
            else
            {
                obj = GetValue<object>(keyName, null);
            }
            return (T)obj;
        }

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
        /// Sets a properties value in the internal property store.
        /// If a null is passed the property will be reset and removed.
        /// </summary>
        /// <param name="keyName">Property name of whos value is to be set</param>
        /// <param name="value">Value of the property</param>
        protected void SetValue(string keyName, object value)
        {
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

            if (PropertyChanged != null)
            {
                string propertyName =
                    new Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Returns true if the property has been set
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
            if (_rowstate == DataRowState.Deleted)
                throw new ApplicationException("Object has been marked for deletion and can not be modified.");

            if (_rowstate == DataRowState.Unchanged || _rowstate == DataRowState.Added)
            {
                _rowstateOriginal = _rowstate;
                oldDataValue = (IDictionary<string, object>)binarySerialiseClone(dataValue);
                oldForeignKeys = (IDictionary<string, object>)binarySerialiseClone(foreignKeys);
                _rowstate = DataRowState.Modified;
            }
        }

        /// <summary>
        /// Accepts the current state of the object
        /// </summary>
        public virtual void AcceptChanges()
        {
            _rowstateOriginal = _rowstate;
            oldDataValue = null;
            oldForeignKeys = null;
        }

        /// <summary>
        /// Reverts any changes back to the last accepted state
        /// </summary>
        public virtual void RejectChanges()
        {
            if (oldDataValue != null)
            {
                _rowstate = _rowstateOriginal;
                dataValue = oldDataValue;
                foreignKeys = oldForeignKeys;
            }
        }
        #endregion

        #region Persistance Methods (Save, Delete)
        /// <summary>
        /// Saves this business object to the persisted medium.
        /// </summary>
        public virtual void Save()
        {
            QueryAction action = GetPersistanceQueryAction();
            if (action == QueryAction.Delete)
            {
                if(OnDeleting != null)
                    OnDeleting(this, new EventArgs());
                NHibernateSessionProvider.Provider.CurrentSession.Delete(this);
                if(OnDeleted != null)
                    OnDeleted(this, new EventArgs());
            }
            else if (action == QueryAction.Insert)
            {
                if(OnSaving != null)
                    OnSaving(this, new EventArgs());
                NHibernateSessionProvider.Provider.CurrentSession.Save(this);
                if(OnSaved != null)
                    OnSaved(this, new EventArgs());
            }
            else if (action == QueryAction.Update)
            {
                
                if (OnSaving != null)
                    OnSaving(this, new EventArgs());
                NHibernateSessionProvider.Provider.CurrentSession.Update(this);
                NHibernateSessionProvider.Provider.CurrentSession.Flush();
                if (OnSaved != null)
                    OnSaved(this, new EventArgs());
            }

            SetSaveRowState();
        }

        /// <summary>
        /// Sets the state of this business object to deleted. Call Save() to update the persisted medium
        /// </summary>
        public virtual void Delete()
        {
            _rowstate = DataRowState.Deleted;
        }

        /// <summary>
        /// Sets the business object's RowState to Loaded
        /// </summary>
        protected virtual internal void SetLoadRowState()
        {
            _rowstate = DataRowState.Unchanged;
        }

        /// <summary>
        /// Returns the query action needed to save the current state of the object
        /// </summary>
        public virtual QueryAction GetPersistanceQueryAction()
        {
            QueryAction action = QueryAction.None;
            if (_rowstate == DataRowState.Detached)
            {
                action = QueryAction.Insert;
                _rowstate = DataRowState.Unchanged;
            }
            else if (_rowstate == DataRowState.Modified)
            {
                action = QueryAction.Update;
                _rowstate = DataRowState.Unchanged;
            }
            else if (_rowstate == DataRowState.Deleted)
            {
                action = QueryAction.Delete;
            }
            else if (_rowstate == DataRowState.Added)
            {
                action = QueryAction.Insert;
                _rowstate = DataRowState.Unchanged;
            }
            else if (_rowstate == DataRowState.Unchanged)
            {
                action = QueryAction.Update;
            }
            return action;
        }

        /// <summary>
        /// Sets the business object's rowstate to indicate the object has been saved
        /// </summary>
        /// <returns></returns>
        protected virtual internal QueryAction SetSaveRowState()
        {
            QueryAction action = GetPersistanceQueryAction();
            AcceptChanges();
            return action;
        }

        /// <summary>
        /// Loads a business object with the given ID
        /// </summary>
        public static DataObject Load(int Id)
        {
            throw new NotImplementedException("Load not implemented at abstract DataObject level");
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Clone a deep-copy of this object
        /// </summary>
        public virtual object Clone()
        {
            return binarySerialiseClone(this);
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        internal static object binarySerialiseClone(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter BinaryFormat = new BinaryFormatter();
            BinaryFormat.Serialize(stream, obj);
            stream.Position = 0;
            return BinaryFormat.Deserialize(stream);
        }
        #endregion

        #region IEditableObject Members

        void IEditableObject.BeginEdit()
        {
            BeginEdit();
        }

        void IEditableObject.CancelEdit()
        {
            RejectChanges();
        }

        void IEditableObject.EndEdit()
        {
            AcceptChanges();
        }

        #endregion

        #region Validation()
        public virtual ValidationRuleCollection ValidationRules
        {
            get
            {
                if (validationRules.Count == 0)
                {
                    AddValidationRules();

                    //DatabaseFieldAttribute attrib = null;
                    //foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this))
                    //{
                    //    attrib = (DatabaseFieldAttribute)prop.Attributes[typeof(DatabaseFieldAttribute)];
                    //    if (attrib != null && attrib.LengthSpecified)
                    //    {
                    //        ValidationRule rule = new ValidationRule(GeneralAssertionTemplate.LengthLess(this, prop.Name, attrib.Length));
                    //        validationRules.Add(rule);
                    //    }
                    //}
                }

                return validationRules;
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
                    return ValidationRules.Messages[0];
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
                if (ValidationRules.Validate())
                    return string.Empty;

                string findError = string.Empty;
                foreach (ValidationRule r in ValidationRules)
                {
                    if (r.PropertyName == columnName && !r.IsValid)
                        findError = r.Message;
                }

                return findError;
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

    /// <summary>
    /// An abstract class that provides core business object funcationality.
    /// <remarks>
    /// Core functionality targets specifically:
    /// 1. tracking/cloning an object's persistance state
    /// 2. providing basic persistance functionality
    /// 3. implementing the built-in binding interfaces for use with .net's built-in datacontrols (ie. GridView)
    /// </remarks>
    /// </summary>
    [Serializable]
    public abstract class DataObject<T> : DataObject
        where T : DataObject
    {
        /// <summary>
        /// Clone a deep-copy of this object
        /// </summary>
        public new virtual T Clone()
        {
            return (T)binarySerialiseClone(this);
        }

        /// <summary>
        /// Loads a business object with the given ID
        /// </summary>
        public static new T Load(int Id)
        {
            T newObj = NHibernateSessionProvider.Provider.CurrentSession.Get<T>(Id);
            newObj.SetLoadRowState();
            return newObj;
        }

        /// <summary>
        /// Gets a strongly typed list of business objects based on NHibernate criteria
        /// </summary>
        public static IList<T> Search(NHibernate.ICriteria criteria)
        {
            IList<T> list = criteria.List<T>();
            foreach(T item in list)
                item.SetLoadRowState();
            return list;
        }

        /// <summary>
        /// Gets a strongly typed list of business objects based on an NHibernate Query
        /// </summary>
        public static IList<T> Search(NHibernate.IQuery query)
        {
            IList<T> list = query.List<T>();
            foreach (T item in list)
                item.SetLoadRowState();
            return list;
        }
    }
}
