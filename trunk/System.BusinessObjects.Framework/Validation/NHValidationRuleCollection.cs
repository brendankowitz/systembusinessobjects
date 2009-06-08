using System;
using System.Collections.Generic;
using System.Text;
using System.BusinessObjects.Data;

#if NH_VALIDATOR
using NHibernate.Validator.Engine;

namespace System.BusinessObjects.Validation
{
    [Serializable]
    public class NHValidationRuleCollection : IValidationRuleCollection
    {
        NHibernate.Validator.Engine.ClassValidator validator = null;
        DataObject _parent = null;
        InvalidValue[] _invalidValues = null;
        internal bool isDirty = true;

        public NHValidationRuleCollection(DataObject parent)
        {
            validator = new NHibernate.Validator.Engine.ClassValidator(parent.GetType());
            _parent = parent;
            parent.PropertyChanged += parent_PropertyChanged;
        }

        void parent_PropertyChanged(object sender, ComponentModel.PropertyChangedEventArgs e)
        {
            MarkDirty();
        }        

        #region IValidationRuleCollection Members

        public void MarkDirty()
        {
            isDirty = true;
        }

        public bool Validate()
        {
            if(isDirty)
                _invalidValues = validator.GetInvalidValues(_parent);
            if (_invalidValues == null) return true;
            return false;
        }

        public bool IsValid
        {
            get { return Validate(); }
        }

        public string[] Messages
        {
            get 
            {
                Validate();
                List<string> messages = new List<string>();
                if (_invalidValues != null)
                {
                    foreach (InvalidValue val in _invalidValues)
                    {
                        messages.Add(val.Message);
                    }
                }
                return messages.ToArray();
            }
        }

        public int Count
        {
            get { return 0; }
        }

        public void Add(object validationRule)
        {
            throw new NotImplementedException("Please use class attributes or NHValidator xml to configure validation rules.");
        }

        /// <summary>
        /// Adds validation rules based on attributes
        /// </summary>
        public void AddRuleFromAttributes(PropertyDescriptor property, AttributeCollection collection)
        {
            
        }

        #endregion

#region IDataErrorInfo Members

        public string Error
        {
            get
            {
                if (_invalidValues != null && _invalidValues.Length > 0)
                {
                    return _invalidValues[0].Message;
                }
                return null;
            }
        }

        public string this[string columnName]
        {
            get 
            {
                InvalidValue[] values = validator.GetInvalidValues(_parent, columnName);
                if (values != null && values.Length > 0)
                    return values[0].Message;
                return null;
            }
        }

        #endregion

#region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
#endif