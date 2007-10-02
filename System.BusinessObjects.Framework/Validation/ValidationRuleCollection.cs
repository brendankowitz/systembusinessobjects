using System.Collections.Generic;
using System.BusinessObjects.Data;

namespace System.BusinessObjects.Validation
{
    [Serializable]
    public class ValidationRuleCollection : List<ValidationRule>
    {
        string[] messages = new string[] { };
        bool isvalid = false;
        internal bool isDirty = true;

        public ValidationRuleCollection()
        {
            
        }

        public ValidationRuleCollection(DataObject parent)
        {
            parent.PropertyChanged += parent_PropertyChanged;
        }

        void parent_PropertyChanged(object sender, ComponentModel.PropertyChangedEventArgs e)
        {
            isDirty = true;
        }

        public void MarkDirty()
        {
            isDirty = true;
        }

        /// <summary>
        /// Validates the entire list
        /// </summary>
        public bool Validate()
        {
            if (isDirty)
            {
                isvalid = true;
                List<string> message = new List<string>();
                for (int i = 0; i < Count; i++)
                {
                    if (!this[i].Validate())
                    {
                        isvalid = false;
                        message.Add(this[i].Message);
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
    }
}
