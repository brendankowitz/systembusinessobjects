using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.BusinessObjects.Data
{
    /// <summary>
    /// A base class that provides simple comparison of generic objects, based on specified fields.
    /// </summary>
    /// <typeparam name="T"> The type of the objects to be compared. </typeparam>
    public class GenericObjectComparer<T> : IComparer<T>
    {
        #region Properties

        private string _direction = "ASC";

        /// <summary>
        /// The direction to compare or sort (ASC/DESC).
        /// </summary>
        public string Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        private string _fieldToCompare;

        /// <summary>
        /// The field to compare.
        /// </summary>
        public string CompareField
        {
            get { return _fieldToCompare; }
            set { _fieldToCompare = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor requiring the field we're comparing and the sort direction.
        /// </summary>
        /// <param name="fieldToCompare"> The field to compare. </param>
        /// <param name="direction"> The direction of the sort, ASC or DESC. </param>
        public GenericObjectComparer(string fieldToCompare, string direction)
        {
            _fieldToCompare = fieldToCompare;
            _direction = direction;
        }

        #endregion

        #region IComparer<T> Members

        public int Compare(T x, T y)
        {
            //Split the fields into an array. Delimited by dots.           
            string[] fieldParts = _fieldToCompare.Split('.');

            object compareValX = x;
            object compareValY = y;

            // Get the property value of each field in our list
            foreach (string field in fieldParts)
            {
                if (compareValX != null)
                    compareValX = GetPropertyValue(compareValX, field);
                if (compareValY != null)
                    compareValY = GetPropertyValue(compareValY, field);
            }

            //Handle null values
            int compareValue;
            if (compareValX == null && compareValY == null)
                compareValue = 0;
            else if (compareValX == null && compareValY != null)
                compareValue = -1;
            else if (compareValX != null && compareValY == null)
                compareValue = 1;
            else
                compareValue = ((IComparable)compareValX).CompareTo(compareValY);

            // Quick switch of the result if we're descending
            if (_direction.ToUpper() == "DESC")
                compareValue = -1 * compareValue;

            return compareValue;
        }

        private object GetPropertyValue(object o, string property)
        {
            PropertyInfo propInfo = o.GetType().GetProperty(property);
            object val = propInfo.GetValue(o, null);
            return val;
        }

        #endregion
    }
}
