using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System.BusinessObjects.Validation;
using System.BusinessObjects.Transactions;
using System.Collections;
using System.Diagnostics;
using System.Xml.Serialization;
using System.BusinessObjects.Helpers;

namespace System.BusinessObjects.Data
{
    /// <summary>
    /// An abstract class that provides core business object functionality.
    /// </summary>
    /// <remarks>
    /// Core functionality targets specifically:
    /// 1. tracking/cloning an object's persistance state
    /// 2. providing basic persistance functionality
    /// 3. implementing the built-in binding interfaces for use with .net's built-in datacontrols (ie. GridView)
    /// </remarks>
    [Serializable]
    public abstract class DataObject : ICloneable, IEditableObject,
        IDataErrorInfo
    {
        #region Events
        public virtual event PropertyChangedEventHandler PropertyChanged;
        public virtual event EventHandler OnDeleting;
        public virtual event EventHandler OnDeleted;
        public virtual event EventHandler OnSaving;
        public virtual event EventHandler OnSaved;
        #endregion

        #region Protected Variables
        protected IDictionary<string, object> dataValue = new Dictionary<string, object>();
        protected IDictionary<string, bool> collectionLoaded = new Dictionary<string, bool>();
        [NonSerialized, XmlIgnore]
        protected DataRowState _rowstateOriginal;
        protected DataRowState _rowstate = DataRowState.Detached;
        [NonSerialized, XmlIgnore]
        protected ValidationRuleCollection validationRules;
        [NonSerialized, XmlIgnore]
        private bool _autoFlush = true;
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

        /// <summary>
        /// Specifies if this object should automatically flush changes to the datastore as they are called
        /// </summary>
        [XmlIgnore]
        public virtual bool AutoFlush
        {
            get { return _autoFlush; }
            set{ _autoFlush = value; }
        }
        #endregion

        #region Enum QueryAction
        /// <summary>
        /// The type of query needed to persist this business object
        /// </summary>
        public enum QueryAction
        {
            /// <summary>
            /// Indicates that the object should be inserted
            /// </summary>
            Insert,
            /// <summary>
            /// Indicates that the object should be updated
            /// </summary>
            Update,
            /// <summary>
            /// Indicates that the object should be deleted
            /// </summary>
            Delete,
            /// <summary>
            /// Indicates that no action is required, this object is up-to-date
            /// </summary>
            None
        }
        #endregion

        #region .ctor
        public DataObject()
        {
            validationRules = new ValidationRuleCollection(this);
        }
        #endregion

        #region Get / Set Properties

        /// <summary>
        /// Gets a value from the internal data store
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="keyName">Key of the object to fetch</param>
        /// <returns>the object's value</returns>
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
                string propertyName = new Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
                if (!string.IsNullOrEmpty(propertyName) && propertyName.Length > 3)
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(propertyName.Substring(4)));
                }
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

        /// <summary>
        /// Returns true the first time this function is hit. After this the collection is assumed to have been loaded.
        /// </summary>
        protected virtual bool CollectionLoaded(string collectionName, IEnumerable collection)
        {
            if(collectionLoaded.ContainsKey(collectionName))
            {
                return false;
            }
            else
            {
                SetLoadedRowState(collection);
                collectionLoaded.Add(collectionName, true);
                return true;
            }
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
                _rowstate = DataRowState.Modified;
            }
        }

        /// <summary>
        /// Accepts the current state of the object
        /// </summary>
        public virtual void AcceptChanges()
        {
            _rowstateOriginal = _rowstate;
        }

        /// <summary>
        /// Reverts any changes back to the last accepted state
        /// </summary>
        public virtual void RejectChanges()
        {
            //if (oldDataValue != null)
            //{
            Refresh();
                //dataValue = oldDataValue;
                //foreignKeys = oldForeignKeys;
            //}
        }

        /// <summary>
        /// Refreshes the object state from the datastore
        /// </summary>
        public virtual void Refresh()
        {
            UnitOfWork.CurrentSession.Refresh(this);
            _rowstate = _rowstateOriginal;
            collectionLoaded.Clear();
        }
        #endregion

        #region Persistance Methods (Save, Delete)
        /// <summary>
        /// Saves this business object to the datastore.
        /// </summary>
        public virtual void Save()
        {
            QueryAction action = GetPersistanceQueryAction();
            if (action == QueryAction.Delete)
            {
                if(OnDeleting != null)
                    OnDeleting(this, new EventArgs());
                UnitOfWork.CurrentSession.Delete(this);
                if(OnDeleted != null)
                    OnDeleted(this, new EventArgs());
            }
            else if (action == QueryAction.Insert)
            {
                if(OnSaving != null)
                    OnSaving(this, new EventArgs());
                UnitOfWork.CurrentSession.Save(this);
                if(OnSaved != null)
                    OnSaved(this, new EventArgs());
            }
            else if (action == QueryAction.Update)
            {
                
                if (OnSaving != null)
                    OnSaving(this, new EventArgs());
                UnitOfWork.CurrentSession.Update(this);
                if (AutoFlush)
                    UnitOfWork.CurrentSession.Flush();
                if (OnSaved != null)
                    OnSaved(this, new EventArgs());
            }

            SetSaveRowState();
        }

        /// <summary>
        /// Sets the state of this business object to deleted. Call Save() to update the datastore
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
        /// Marks the BusinessObjects in an enumeration as Loaded
        /// </summary>
        public static void SetLoadedRowState(IEnumerable list)
        {
            if (list != null)
            {
                IEnumerator e = list.GetEnumerator();
                while (e.MoveNext())
                {
                    ((DataObject) e.Current).SetLoadRowState();
                }
            }
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

        /// <summary>
        /// Loads a business object with the given ID
        /// </summary>
        public static T Load<T>(int Id) where T : DataObject
        {
            T newObj = UnitOfWork.CurrentSession.Get<T>(Id);
            if (newObj != null)
                newObj.SetLoadRowState();
            return newObj;
        }

        /// <summary>
        /// Returns a list of all business objects of this type
        /// </summary>
        public static IList<T> Search<T>() where T : DataObject
        {
            NHibernate.ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(T));
            IList<T> list = qry.List<T>();
            SetLoadedRowState(list);
            return list;
        }

        /// <summary>
        /// Returns a list of all business objects of this type
        /// </summary>
        public static IList<T> Search<T>(NHibernate.Expression.Order orderBy) where T : DataObject
        {
            NHibernate.ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(T));
            qry.AddOrder(orderBy);
            IList<T> list = qry.List<T>();
            SetLoadedRowState(list);
            return list;
        }

        /// <summary>
        /// Gets a strongly typed list of business objects based on NHibernate criteria
        /// </summary>
        public static IList<T> Search<T>(NHibernate.ICriteria criteria) where T : DataObject
        {
            IList<T> list = criteria.List<T>();
            SetLoadedRowState(list);
            return list;
        }

        /// <summary>
        /// Gets a strongly typed list of business objects based on an NHibernate Query
        /// </summary>
        public static IList<T> Search<T>(NHibernate.IQuery query) where T : DataObject
        {
            IList<T> list = query.List<T>();
            SetLoadedRowState(list);
            return list;
        }

        /// <summary>
        /// Gets a strongly typed business object based on NHibernate criteria
        /// </summary>
        public static T Fetch<T>(NHibernate.ICriteria criteria) where T : DataObject
        {
            T item = criteria.UniqueResult<T>();
            if (item != null)
                item.SetLoadRowState();
            return item;
        }

        /// <summary>
        /// Gets a strongly typed business object based on an NHibernate Query
        /// </summary>
        public static T Fetch<T>(NHibernate.IQuery query) where T : DataObject
        {
            T item = query.UniqueResult<T>();
            if (item != null)
                item.SetLoadRowState();
            return item;
        }

        /// <summary>
        /// Evicts an existing instance of this business object from NHibernate's session cache
        /// </summary>
        public static void Evict<T>(int ID) where T : DataObject
        {
            T existingObj = UnitOfWork.CurrentSession.Get<T>(ID);
            if (existingObj != null)
            {
                existingObj.Evict();
            }
        }

        /// <summary>
        /// Evicts the current object from NHibernate's session cache
        /// </summary>
        public virtual void Evict()
        {
            if (UnitOfWork.CurrentSession != null)
                UnitOfWork.CurrentSession.Evict(this);
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

        //[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
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
        /// <summary>
        /// A collection of validation rules used to determine if the data in the object is valid
        /// </summary>
        [XmlIgnore]
        public virtual ValidationRuleCollection ValidationRules
        {
            get
            {
                if (validationRules.Count == 0)
                {
                    AddValidationRules();

                    ValidationLengthAttribute lengthAttrib;
                    ValidationNotEmptyAttribute notEmptyAttrib;
                    foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this))
                    {
                       lengthAttrib = (ValidationLengthAttribute)prop.Attributes[typeof(ValidationLengthAttribute)];
                        notEmptyAttrib = (ValidationNotEmptyAttribute)prop.Attributes[typeof(ValidationNotEmptyAttribute)];
                        if (lengthAttrib != null)
                        {
                            if (lengthAttrib._maxLengthSpecified)
                            {
                                ValidationRule rule =
                                    new ValidationRule(
                                        GeneralAssertionTemplate.LengthLess(this, prop.Name, lengthAttrib.MaxLength));
                                validationRules.Add(rule);
                            }
                            if(lengthAttrib._minLengthSpecified)
                            {
                                ValidationRule rule =
                                    new ValidationRule(
                                        GeneralAssertionTemplate.LengthGreater(this, prop.Name, lengthAttrib.MinLength));
                                validationRules.Add(rule);
                            }
                        }
                        if (notEmptyAttrib != null)
                        {
                            ValidationRule rule = new ValidationRule(GeneralAssertionTemplate.IsNotEmpty(this, prop.Name));
                            validationRules.Add(rule);
                        }
                    }
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

        #region Xml Serialization

        /// <summary>
        /// Returns an Xml representation of this object
        /// </summary>
        public virtual string SerializeToXml()
        {
            return XmlHelper.SerializeToXML(this);
        }

        /// <summary>
        /// Reinstantiates an object from Xml
        /// </summary>
        public virtual T DeserializeFromXml<T>(string xml)
        {
            return XmlHelper.Deserialise<T>(xml);
        }

        #endregion
    }
}
