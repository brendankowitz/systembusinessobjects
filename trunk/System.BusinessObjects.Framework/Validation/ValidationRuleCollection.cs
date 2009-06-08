using System.Collections.Generic;
using System.BusinessObjects.Data;
using System.Collections;
using System.ComponentModel;

#if DOT_NET_35
using System.ComponentModel.DataAnnotations;
#endif

namespace System.BusinessObjects.Validation
{
    /// <summary>
    /// Defines a list of validation rules
    /// </summary>
    [Serializable]
    public class ValidationRuleCollection : IValidationRuleCollection
    {
        List<ValidationRule> rules = new List<ValidationRule>();
        string[] messages = new string[] { };
        bool isvalid = false;
        internal bool isDirty = true;
        WeakReference parentObject;

        /// <summary>
        /// .ctor for a new ValidationRuleCollection
        /// </summary>
        public ValidationRuleCollection()
        {
            
        }

        /// <summary>
        /// .ctor for a new ValidationRuleCollection specifying the parent of whos properties to watch for change events
        /// </summary>
        public ValidationRuleCollection(DataObject parent)
        {
            parentObject = new WeakReference(parent);
            parent.PropertyChanged += parent_PropertyChanged;
        }

        void parent_PropertyChanged(object sender, ComponentModel.PropertyChangedEventArgs e)
        {
            isDirty = true;
        }

        /// <summary>
        /// Adds a new Validation Rule
        /// </summary>
        public void Add(object rule)
        {
            rules.Add(rule as ValidationRule);
        }

        /// <summary>
        /// Adds validation rules based on attributes
        /// </summary>
        public void AddRulesFromAttributes(PropertyDescriptor property, AttributeCollection collection)
        {
            foreach (Attribute customAtt in collection)
            {
                if (customAtt is ValidationBaseAttribute)
                {
                    ValidationBaseAttribute attrib = (ValidationBaseAttribute)customAtt;
                    IList<ValidationRule> tRules = attrib.GetValidationRules(parentObject.Target as DataObject, property.Name);
                    if (tRules != null && tRules.Count > 0)
                    {
                        foreach (ValidationRule r in tRules)
                            Add(r);
                    }
                }
#if DOT_NET_35
                else if (customAtt is ValidationAttribute)
                {
                    ValidationAttribute innerAttribute = customAtt as ValidationAttribute;
                    if (innerAttribute != null)
                    {
                        ValidatorTemplate frameworkRule = delegate(out string propName, out string message)
                        {
                            bool retval = true;
                            propName = property.Name;
                            message = string.Empty;
                            try
                            {   //Let .net3.5 validate the property
                                ((ValidationAttribute)innerAttribute).Validate(property.GetValue(parentObject.Target), property.Name);
                            }
                            catch (ValidationException ex)
                            {
                                retval = false;
                                message = ex.Message;
                            }
                            return retval;
                        };
                        Add(new ValidationRule(frameworkRule));
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Count of validation rules in the collection
        /// </summary>
        public int Count
        {
            get
            {
                return rules.Count;
            }
        }

        /// <summary>
        /// Marks the list as dirty, this will force it to be re-evaluated
        /// </summary>
        public void MarkDirty()
        {
            isDirty = true;
        }

        /// <summary>
        /// Validates the entire list and returns false if there has been an error
        /// </summary>
        public bool Validate()
        {
            if (isDirty)
            {
                isvalid = true;
                List<string> message = new List<string>();
                for (int i = 0; i < rules.Count; i++)
                {
                    if (!rules[i].Validate())
                    {
                        isvalid = false;
                        message.Add(rules[i].Message);
                    }
                }
                messages = message.ToArray();
            }
            return isvalid;
        }

        /// <summary>
        /// Returns if the entire list is valid
        /// </summary>
        public bool IsValid
        {
            get
            {
                return isvalid;
            }
        }

        /// <summary>
        /// Returns any error messages from the validation of the list
        /// </summary>
        public string[] Messages
        {
            get
            {
                return messages;
            }
        }
    
        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
	        get 
            {
                if (Validate())
                    return string.Empty;
                return Messages[0];
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
	        get 
            {
                if (Validate())
                    return string.Empty;

                string findError = string.Empty;
                foreach (ValidationRule r in rules)
                {
                    if (r.PropertyName == columnName && !r.IsValid)
                    {
                        findError = r.Message;
                        break;
                    }
                }
                return findError;
            }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return rules.GetEnumerator();
        }

        #endregion
    }
}
