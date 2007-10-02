using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.BusinessObjects.Helpers;
using System.BusinessObjects.Providers;

namespace System.BusinessObjects.Controls
{
    [NonVisualControl]
    public class BindableDropDownListExtender : WebControl
    {
        private string _attachTo;
        /// <summary>
        /// Specifies the ObjectDataSource to attach to
        /// </summary>
        public string AttachTo
        {
            get { return _attachTo; }
            set { _attachTo = value; }
        }

        public object BoundValue
        {
            get
            {
                if(attachToControl != null && attachToControl.SelectedIndex > -1)
                {
                    return NHibernateSessionProvider.Provider.CurrentSession.Get(boundType,
                                                                          int.Parse(attachToControl.SelectedValue));
                }
                return null;
            }
            set 
            {
                if (value != null)
                {
                    foreach(PropertyDescriptor info in TypeDescriptor.GetProperties(value))
                    {
                        if (info.Name == attachToControl.DataValueField && info.GetValue(value) != null)
                        {
                            string id = info.GetValue(value).ToString();
                            attachToControl.SelectedValue = id;
                            break;
                        }
                    }
                }
                else
                {
                    attachToControl.SelectedIndex = -1;   
                }
            }
        }

        private string _type;

        public string BoundValueType
        {
            get { return _type; }
            set { _type = value; }
        }

        protected Type boundType
        {
            get
            {
                return Type.GetType(_type);
            }
        }


        private DropDownList attachToControl = null;

        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                writer.WriteLine(string.Format("<span>[BindableDropDownListExtender for {0}]</span>", AttachTo));
            }
            else
            {
                base.Render(writer);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            attachToControl = WebHelper.FindNestedWebControl(Page.Controls, AttachTo) as DropDownList;
        }
    }
}
