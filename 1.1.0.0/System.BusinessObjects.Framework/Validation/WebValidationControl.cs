using System.BusinessObjects.Helpers;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace System.BusinessObjects.Validation
{
    [NonVisualControl]
    public class WebValidationControlExtender : WebControl
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

        private string _propertyToValidate;
        /// <summary>
        /// Optionally. Specifies only one property to validate
        /// </summary>
        public string PropertyToValidate
        {
            get { return _propertyToValidate; }
            set { _propertyToValidate = value; }
        }


        private Control attachToControl = null;

        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                writer.WriteLine(string.Format("<span>[WebValidationControl for {0}]</span>", AttachTo));
            }
            else
            {
                base.Render(writer);
            }
        }
    
        /// <summary>
        /// Accessing controls such as LoginView on the OnInit event causes them not to
        /// render correctly. This is why the events are now hooked up during onload.
        /// </summary>
        protected override void  OnLoad(EventArgs e)
        {
 	        base.OnLoad(e);

            attachToControl = WebHelper.FindNestedWebControl(Page.Controls, AttachTo);
 
            if(attachToControl != null) //hook up events
            {
                if (attachToControl.GetType() == typeof(ObjectDataSource))
                {
                    ((ObjectDataSource)attachToControl).Inserting += WebValidationControlExtender_Inserting;
                    ((ObjectDataSource)attachToControl).Updating += WebValidationControlExtender_Updating;
                }
                else
                {
                    throw new ApplicationException(string.Format("WebValidationControlExtender is not compatable with {0} controls", attachToControl.GetType()));
                }
            }
        }

        void WebValidationControlExtender_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            if(!ValidateObjectCollection(e.InputParameters.Values))
                e.Cancel = true;
        }

        void WebValidationControlExtender_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            if (!ValidateObjectCollection(e.InputParameters.Values))
                e.Cancel = true;
        }

        protected bool ValidateObjectCollection(ICollection collection)
        {
            bool success = true;
            foreach (object obj in collection)
            {
                if (typeof(IDataErrorInfo).IsAssignableFrom(obj.GetType()))
                {
                    if (!string.IsNullOrEmpty(PropertyToValidate))
                    {
                        string errorMessage = ((IDataErrorInfo)obj)[PropertyToValidate];
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            AddErrorControl(errorMessage, PropertyToValidate);
                            success = false;
                        }
                    }
                    else
                    {
                        foreach(PropertyDescriptor info in TypeDescriptor.GetProperties(obj))
                        {
                            string errorMessage = ((IDataErrorInfo)obj)[info.Name];
                            if (!string.IsNullOrEmpty(errorMessage))
                            {
                                AddErrorControl(errorMessage, info.Name);
                                success = false;
                            }
                        }
                    }
                }
            }
            return success;
        }

        protected void AddErrorControl(string message, string propertyName)
        {
            CustomValidator validator = new CustomValidator();
            validator.ErrorMessage = message;
            validator.IsValid = false;

            Page.Validators.Add(validator);

            foreach (DetailsView dv in WebHelper.FindNestedWebControls(Page.Controls, typeof(DetailsView)))
            {
                if (dv.DataSourceID == attachToControl.ID)
                {
                    foreach (DataControlFieldCell cell in WebHelper.FindNestedWebControls(dv.Controls, typeof(DataControlFieldCell)))
                    {
                        if (cell.ContainingField is BoundField && ((BoundField)cell.ContainingField).DataField == propertyName && cell.Controls.Count > 0)
                        {
                            Literal errorMsg = new Literal();
                            errorMsg.Text = string.Format("<span style=\"color:red;font-weight:bold;\" title=\"{0}\">*</span>", message);
                            cell.Controls.Add(errorMsg);
                        }
                    }
                }
            }
        }
    }
}
