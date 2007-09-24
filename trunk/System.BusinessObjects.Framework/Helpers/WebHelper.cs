using System.Web.UI;
using System.Collections.Generic;

namespace System.BusinessObjects.Helpers
{
    public class WebHelper
    {
        public static Control FindNestedWebControl(ControlCollection controls, string controlId)
        {
            Control foundControl = null;
            foreach(Control c in controls)
            {
                if(c.ID == controlId)
                {
                    foundControl = c;
                    break;
                }
                if(c.Controls != null && c.Controls.Count > 0)
                {
                    foundControl = FindNestedWebControl(c.Controls, controlId);
                    if(foundControl != null)
                        break;
                }
            }
            return foundControl;
        }

        public static Control FindNestedWebControlByLike(ControlCollection controls, string controlIdIsLike)
        {
            Control foundControl = null;
            foreach (Control c in controls)
            {
                if (!string.IsNullOrEmpty(c.ID) && c.ID.Contains(controlIdIsLike))
                {
                    foundControl = c;
                    break;
                }
                if (c.Controls != null && c.Controls.Count > 0)
                {
                    foundControl = FindNestedWebControlByLike(c.Controls, controlIdIsLike);
                    if (foundControl != null)
                        break;
                }
            }
            return foundControl;
        }

        public static List<Control> FindNestedWebControls(ControlCollection controls, Type type)
        {
            List<Control> foundControls = new List<Control>();
            foreach (Control c in controls)
            {
                if (c.GetType() == type)
                {
                    foundControls.Add(c);
                }
                if (c.Controls != null && c.Controls.Count > 0)
                {
                    foundControls.AddRange(FindNestedWebControls(c.Controls, type));
                }
            }
            return foundControls;
        }
    }
}
