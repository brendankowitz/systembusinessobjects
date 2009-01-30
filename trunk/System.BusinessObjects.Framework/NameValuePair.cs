using System.BusinessObjects.Data;

namespace System.BusinessObjects
{
    [Obsolete("Obsolete in favour of built-in class KeyValuePair<T,R>", true)]
    public class NameValuePair : DataObject
    {
        public NameValuePair()
        {
        }

        public NameValuePair(string name, object val)
        {
            Name = name;
            Value = val;
        }

        public string Name
        {
            get { return GetValue<string>("Name"); }
            set
            {
                BeginEdit();
                SetValue("Name", value);
            }
        }

        public object Value
        {
            get { return GetValue<object>("Value"); }
            set
            {
                BeginEdit();
                SetValue("Value", value);
            }
        }
    }
}
