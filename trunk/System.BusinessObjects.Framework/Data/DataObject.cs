using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Collections;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using System.BusinessObjects.Validation;
using System.BusinessObjects.Transactions;
using System.BusinessObjects.Helpers;

using NHibernate.Impl;
using NHibernate;

//////If .net 2.0 SP1 is not installed, PropertyChangingEventArgs is not defined...
//////
//namespace System.ComponentModel
//{
//    public class PropertyChangingEventArgs : EventArgs
//    {
//        private string _propname;
//        public PropertyChangingEventArgs(string propertyName)
//        {
//            _propname = propertyName;
//        }
//        public virtual string PropertyName { get { return _propname; } }
//    }
//    public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);
//    public interface INotifyPropertyChanging
//    {
//        event PropertyChangingEventHandler PropertyChanging;
//    }
//}

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
        IDataErrorInfo, INotifyPropertyChanged, INotifyPropertyChanging, NHibernate.Classic.IValidatable
    {
        #region Events
        public virtual event PropertyChangingEventHandler PropertyChanging;
        public virtual event PropertyChangedEventHandler PropertyChanged;
        public virtual event EventHandler OnDeleting;
        public virtual event EventHandler OnDeleted;
        public virtual event EventHandler OnSaving;
        public virtual event EventHandler OnSaved;
        #endregion

        #region Protected Variables
        protected IDictionary<string, object> dataValue = new Dictionary<string, object>();
        protected DataRowState _mrowstate = DataRowState.Detached;
        [NonSerialized, XmlIgnore]
        protected DataRowState _rowstateOriginal;
        [NonSerialized, XmlIgnore]
        protected IValidationRuleCollection validationRules;
        [NonSerialized, XmlIgnore]
        private bool _autoFlush = true;
        [NonSerialized, XmlIgnore]
        ISession _entitySession = null;
        #endregion

        #region Private Properties
        private EntityEntry Entry
        {
            get
            {
                return ((SessionImpl)entitySession).GetEntry(this);
            }
        }
        private ISession entitySession
        {
            get
            {
                if (_entitySession == null)
                    return UnitOfWork.CurrentSession;
                return _entitySession;
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The current rowstate of this business object
        /// </summary>
        public virtual DataRowState RowState
        {
            set 
            {
                EntityEntry e = Entry;
                if (e == null)
                    _mrowstate = value;
                else
                {
                    _mrowstate = DataRowState.Detached;
                    switch (value)
                    {
                        case DataRowState.Added:
                        case DataRowState.Detached:
                            e.ExistsInDatabase = false;
                            break;
                        case DataRowState.Deleted:
                            //e.Status = Status.Deleted;
                            //e.ExistsInDatabase = true;
                            //e.DeletedState = e.LoadedState;
                            _mrowstate = DataRowState.Deleted;
                            break;
                        case DataRowState.Modified:
                            e.ExistsInDatabase = true;
                            break;
                        case DataRowState.Unchanged:
                            e.ExistsInDatabase = true;
                            e.Status = Status.Loaded;
                            break;
                    }
                }
            }
            get
            {
                if (_mrowstate != DataRowState.Detached)
                    return _mrowstate;

                EntityEntry e = Entry;
                if (e != null)
                {
                    switch (e.Status)
                    {
                        case Status.Deleted:
                        case Status.Gone:
                            return DataRowState.Deleted;
                        case Status.Loading:
                        case Status.Loaded:
                            try
                            {
                                int[] changes =
                                    e.Persister.FindDirty(e.LoadedState, e.Persister.GetPropertyValues(this), this,
                                                          ((SessionImpl)entitySession).
                                                              GetSessionImplementation());
                                if (changes == null)
                                    return DataRowState.Unchanged;
                                else
                                    return DataRowState.Modified;
                            }catch
                            {
                                return DataRowState.Unchanged;
                            }
                            
                        default:
                            if (e.ExistsInDatabase)
                                return DataRowState.Modified;
                            else
                                return DataRowState.Detached;
                    }
                }
                else
                {
                    return DataRowState.Detached;
                }
            }
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
        /// <summary>
        /// Initializes a new instance of DataObject
        /// </summary>
        public DataObject()
        {
            validationRules = new ValidationRuleCollection(this);
        }

        /// <summary>
        /// Initializes a new instance of DataObject with a session
        /// </summary>
        public DataObject(ISession session) : this()
        {
            _entitySession = session;
        }

        /// <summary>
        /// Sets the default session this business object should use
        /// </summary>
        public virtual void SetSession(ISession session)
        {
            _entitySession = session;
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

            if (typeof(T) == typeof(bool))
            {
                obj = GetValue(keyName, false);
            }
            else if (typeof(T).IsPrimitive) //should cover Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Char, Double, and Single.
            {
                if (typeof(T) == typeof(Byte) || typeof(T) == typeof(UInt16) || typeof(T) == typeof(UInt32) || 
                    typeof(T) == typeof(UInt64) || typeof(T) == typeof(char))
                    obj = GetValue(keyName, default(T));
                else
                    obj = GetValue(keyName, -1);
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
            //Fire property changing event
            string propertyName = string.Empty;
            if(PropertyChanged != null || PropertyChanging != null)
                propertyName = new Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
            if (PropertyChanging != null && !string.IsNullOrEmpty(propertyName) && propertyName.Length > 3)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName.Substring(4)));
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
            if (PropertyChanged != null && !string.IsNullOrEmpty(propertyName) && propertyName.Length > 3)
            {
               OnPropertyChanged(new PropertyChangedEventArgs(propertyName.Substring(4)));
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
            DataRowState state = RowState;
            if (state == DataRowState.Deleted)
                throw new ApplicationException("Object has been marked for deletion and can not be modified.");

            if (state == DataRowState.Unchanged || state == DataRowState.Added)
            {
                _rowstateOriginal = RowState;
                _mrowstate = DataRowState.Modified;
            }
        }

        /// <summary>
        /// Accepts the current state of the object
        /// </summary>
        public virtual void AcceptChanges()
        {
            //_rowstateOriginal = RowState;
        }

        /// <summary>
        /// Reverts any changes back to the last accepted state
        /// </summary>
        public virtual void RejectChanges()
        {
            Refresh();
            _mrowstate = DataRowState.Detached;
        }

        /// <summary>
        /// Refreshes the object state from the datastore
        /// </summary>
        public virtual void Refresh()
        {
            entitySession.Refresh(this);
        }
        #endregion

        #region Persistance Methods (Save, Delete)
        /// <summary>
        /// Saves this business object to the datastore.
        /// </summary>
        public virtual void Save()
        {
            QueryAction action = GetPersistanceQueryAction();
            _mrowstate = DataRowState.Detached;

            if (action == QueryAction.Delete)
            {
                if(OnDeleting != null)
                    OnDeleting(this, new EventArgs());
                entitySession.Delete(this);
                if(OnDeleted != null)
                    OnDeleted(this, new EventArgs());
            }
            else if (action == QueryAction.Insert)
            {
                if(OnSaving != null)
                    OnSaving(this, new EventArgs());
                entitySession.Save(this);
                if(OnSaved != null)
                    OnSaved(this, new EventArgs());
            }
            else if (action == QueryAction.Update)
            {
                if (OnSaving != null)
                    OnSaving(this, new EventArgs());
                entitySession.Update(this);
                if (AutoFlush)
                    entitySession.Flush();
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
            RowState = DataRowState.Deleted;
        }

        /// <summary>
        /// Sets the business object's RowState to Loaded
        /// </summary>
        protected virtual internal void SetLoadRowState()
        {
            RowState = DataRowState.Unchanged;
        }

        /// <summary>
        /// Marks the BusinessObjects in an enumeration as Loaded
        /// </summary>
        [Obsolete]
        public static void SetLoadedRowState(IEnumerable list){ /* To be removed */ }

        /// <summary>
        /// Returns the query action needed to save the current state of the object
        /// </summary>
        public virtual QueryAction GetPersistanceQueryAction()
        {
            QueryAction action = QueryAction.None;
            DataRowState _rowstate = RowState;
            if (_rowstate == DataRowState.Detached)
            {
                action = QueryAction.Insert;
            }
            else if (_rowstate == DataRowState.Modified)
            {
                action = QueryAction.Update;
            }
            else if (_rowstate == DataRowState.Deleted)
            {
                action = QueryAction.Delete;
            }
            else if (_rowstate == DataRowState.Added)
            {
                action = QueryAction.Insert;
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
        protected virtual internal void SetSaveRowState()
        {
            AcceptChanges();
            _mrowstate = DataRowState.Detached;
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
            return newObj;
        }

        /// <summary>
        /// Loads a business object with the given ID
        /// </summary>
        public static T Load<T>(int Id, ISession session) where T : DataObject
        {
            T newObj = session.Get<T>(Id);
            newObj._entitySession = session;

            return newObj;
        }

        /// <summary>
        /// Returns a list of all business objects of this type
        /// </summary>
        public static IList<T> Search<T>() where T : DataObject
        {
            NHibernate.ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(T));
            IList<T> list = qry.List<T>();
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
            return list;
        }

        /// <summary>
        /// Gets a strongly typed list of business objects based on NHibernate criteria
        /// </summary>
        public static IList<T> Search<T>(NHibernate.ICriteria criteria) where T : DataObject
        {
            IList<T> list = criteria.List<T>();
            return list;
        }

        /// <summary>
        /// Gets a strongly typed list of business objects based on an NHibernate Query
        /// </summary>
        public static IList<T> Search<T>(NHibernate.IQuery query) where T : DataObject
        {
            IList<T> list = query.List<T>();
            return list;
        }

        /// <summary>
        /// Gets a strongly typed business object based on NHibernate criteria
        /// </summary>
        public static T Fetch<T>(NHibernate.ICriteria criteria) where T : DataObject
        {
            T item = criteria.UniqueResult<T>();
            return item;
        }

        /// <summary>
        /// Gets a strongly typed business object based on an NHibernate Query
        /// </summary>
        public static T Fetch<T>(NHibernate.IQuery query) where T : DataObject
        {
            T item = query.UniqueResult<T>();
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
            if (entitySession != null)
                entitySession.Evict(this);
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
        /// <remarks>
        /// Use 'set' to set your own instance/implementation of IValidationRuleCollection then override AddValidationRules to add your own rules.
        /// </remarks>
        [XmlIgnore]
        public virtual IValidationRuleCollection ValidationRules
        {
            get
            {
                if (validationRules.Count == 0)
                {
                    AddValidationRules();

                    if (validationRules is ValidationRuleCollection)
                    {
                        ValidationLengthAttribute lengthAttrib;
                        ValidationNotEmptyAttribute notEmptyAttrib;
                        ValidationIsNotNullAttribute isNotNull;
                        foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this))
                        {
                            lengthAttrib = (ValidationLengthAttribute)prop.Attributes[typeof(ValidationLengthAttribute)];
                            notEmptyAttrib = (ValidationNotEmptyAttribute)prop.Attributes[typeof(ValidationNotEmptyAttribute)];
                            isNotNull = (ValidationIsNotNullAttribute)prop.Attributes[typeof(ValidationIsNotNullAttribute)];
                            if (lengthAttrib != null)
                            {
                                if (lengthAttrib._maxLengthSpecified)
                                {
                                    ValidationRule rule =
                                        new ValidationRule(
                                            GeneralAssertionTemplate.LengthLess(this, prop.Name, lengthAttrib.MaxLength));
                                    validationRules.Add(rule);
                                }
                                if (lengthAttrib._minLengthSpecified)
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
                            if (isNotNull != null)
                            {
                                ValidationRule rule = new ValidationRule(GeneralAssertionTemplate.IsBusinessObjectNotNull(this, prop.Name));
                                validationRules.Add(rule);
                            }
                        }
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
            if (GetPersistanceQueryAction() == QueryAction.Delete)
                return;

            if (!ValidationRules.Validate())
            {
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
        public static T DeserializeFromXml<T>(string xml)
        {
            return XmlHelper.Deserialise<T>(xml);
        }

        #endregion
    }
}
