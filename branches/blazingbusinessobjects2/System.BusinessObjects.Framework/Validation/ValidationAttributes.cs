using System;
using System.Collections.Generic;
using System.BusinessObjects.Data;
using System.Text.RegularExpressions;

namespace System.BusinessObjects.Validation
{
    /// <summary>
    /// Abstract base attribute giving the definition for providing Validation rules
    /// </summary>
    [Serializable]
    public abstract class ValidationBaseAttribute : Attribute
    {
        public abstract IList<ValidationRule> GetValidationRules(DataObject current, string property);
    }

    /// <summary>
    /// Validates false if the target property is empty or null by checking the DataObject.IsNull() function
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ValidationIsNotNullAttribute : ValidationBaseAttribute
    {
        public override IList<ValidationRule> GetValidationRules(DataObject current, string property)
        {
            IList<ValidationRule> rules = new List<ValidationRule>();
            ValidationRule rule = new ValidationRule(GeneralAssertionTemplate.IsBusinessObjectNotNull(current, property));
            rules.Add(rule);
            return rules;
        }
    }
}
