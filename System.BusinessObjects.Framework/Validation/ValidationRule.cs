using System;

namespace System.BusinessObjects.Validation
{
    /// <summary>
    /// Provides a delegate template for validating properties
    /// </summary>
    /// <param name="propertyName">Name of the property with the validation error</param>
    /// <param name="message">Description of the validation error</param>
    /// <returns>True if the rule is valid, False if it has been broken</returns>
    public delegate bool ValidatorTemplate(out string propertyName, out string message);

    [Serializable]
    public class ValidationRule
    {
        ValidatorTemplate _rule;
        bool isvalid;
        string message;
        string propName;

        public ValidationRule(ValidatorTemplate rule)
        {
            _rule = rule;
            message = string.Empty;
        }

        public string Message
        {
            get { return message; }
        }

        public string PropertyName
        {
            get { return propName; }
        }

        public bool Validate()
        {
            isvalid = _rule(out propName, out message);
            return isvalid;
        }

        public bool IsValid
        {
            get { return isvalid; }
        }
    }
}
