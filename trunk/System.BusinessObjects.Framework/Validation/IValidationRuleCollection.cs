using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace System.BusinessObjects.Validation
{
    /// <summary>
    /// Defines a list of validation rules
    /// </summary>
    public interface IValidationRuleCollection : IDataErrorInfo, IEnumerable
    {
        /// <summary>
        /// Marks the list as dirty, this will force it to be re-evaluated
        /// </summary>
        void MarkDirty();
        /// <summary>
        /// Validates the entire list
        /// </summary>
        bool Validate();
        /// <summary>
        /// Returns if the entire list is valid
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// Returns any error messages from the validation of the list
        /// </summary>
        string[] Messages { get; }
        /// <summary>
        /// Count of validation rules in the collection
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Adds a new Validation Rule
        /// </summary>
        void Add(object validationRule);
    }
}
