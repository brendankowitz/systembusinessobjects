using System;
using System.Collections.Generic;
using System.Text;

namespace System.BusinessObjects.Validation
{
    public class ValidationRuleCollection : List<ValidationRule>
    {
        string[] messages = new string[] { };
        bool isvalid = false;

        /// <summary>
        /// Validates the entire list
        /// </summary>
        public bool Validate()
        {
            isvalid = true;
            List<string> message = new List<string>();
            for (int i = 0; i < this.Count; i++)
            {
                if (!this[i].Validate())
                {
                    isvalid = false;
                    message.Add(this[i].Message);
                }
            }
            messages = message.ToArray();
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
