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

    /// <summary>
    /// Defines a validation rule template against a property
    /// </summary>
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

        /// <summary>
        /// Error message
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Name of the property to be validated
        /// </summary>
        public string PropertyName
        {
            get { return propName; }
        }

        /// <summary>
        /// Validates this rule by running the validation delegate
        /// </summary>
        public bool Validate()
        {
            isvalid = _rule(out propName, out message);
            return isvalid;
        }

        /// <summary>
        /// A value indicating if the rule is valid
        /// </summary>
        public bool IsValid
        {
            get { return isvalid; }
        }
    }
}
