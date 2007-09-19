using System;
using System.Web.UI.WebControls;
using Sample.BusinessObjects.Contacts;
using System.BusinessObjects.Providers;

namespace Sample.UI.AddressBook
{
    public partial class _Default : System.Web.UI.Page
    {
        #region Page Events
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ObjectDataSourceAddress.Inserted += ObjectDataSourceAddress_Inserted;
            ObjectDataSourceAddress.Deleting += ObjectDataSourceAddress_Deleting;
        }
        #endregion

        #region Maintain Edited Person->Address Relationships
        void ObjectDataSourceAddress_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
        {
            Person person = Person.Load(Convert.ToInt32(GridViewPeople.SelectedDataKey[0]));
            person.Addresses.Add((Address)e.ReturnValue);
            person.Save();
            GridViewAddresses.DataBind();
        }

        void ObjectDataSourceAddress_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            Person person = Person.Load(Convert.ToInt32(GridViewPeople.SelectedDataKey[0]));
            
            Address toDelete = null;
            if (e.InputParameters["obj"] != null)
            {
                foreach (Address addr in person.Addresses)
                {
                    if (addr.ID == ((Address)e.InputParameters["obj"]).ID)
                    {
                        toDelete = addr;
                        break;
                    }
                }

                if (toDelete != null)
                {
                    person.Addresses.Remove(toDelete);
                    person.Save();

                    //tell nhibernate that we no longer need to know about this address because it
                    //is not unlinked.
                    NHibernateSessionProvider.Provider.CurrentSession.Evict(toDelete);
                }
            }
        }
       #endregion

        #region Screen Configurations
        private void SetScreenInitial()
        {
            PanelAddPerson.Visible = false;
            GridViewPeople.Visible = true;
            PanelAddAddress.Visible = false;
        }
        private void SetScreenAdd()
        {
            PanelAddPerson.Visible = true;
            GridViewPeople.Visible = false;
            PanelAddAddress.Visible = false;
        }

        private void SetPanelNewAddressInitial()
        {
            GridViewAddresses.Visible = true;
            PanelAddNewAddress.Visible = false;
        }

        private void SetPanelNewAddressAdd()
        {
            GridViewAddresses.Visible = false;
            PanelAddNewAddress.Visible = true;
        }

        private void SetPanelAddressList()
        {
            PanelAddAddress.Visible = true;
        }
        #endregion

        #region Add/Modify Person
        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            SetScreenInitial();
            GridViewPeople.DataBind();
        }

        protected void ButtonCreateNew_Click(object sender, EventArgs e)
        {
            SetScreenAdd();
        }

        protected void DetailsViewPerson_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            SetScreenInitial();
        }

        protected void DetailsViewPerson_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if(e.CommandName == "Cancel")
                SetScreenInitial();
        }
        #endregion

        #region Add/Mofify Address
        protected void GridViewPeople_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
                SetPanelAddressList();
            else if(e.CommandName == "Delete")
                SetScreenInitial();
        }

        protected void ButtonAddAddress_Click(object sender, EventArgs e)
        {
            SetPanelNewAddressAdd();
        }

        protected void DetailsViewAddress_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
                SetPanelNewAddressInitial();
        }

        protected void DetailsViewAddress_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            SetPanelNewAddressInitial();
        }
        #endregion  
    }
}