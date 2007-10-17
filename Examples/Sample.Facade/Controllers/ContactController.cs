using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Sample.BusinessObjects.Contacts;
using Sample.BusinessObjects.Queries;

namespace Sample.Facade.Controllers
{
    [System.ComponentModel.DataObject]
    public class ContactController
    {
        #region Person Members

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<Person> SearchPeopleByName(string name)
        {
            return Person.Search(QrySearchContactByName.Query(name));
        }

        [DataObjectMethod(DataObjectMethodType.Insert)]
        public Person FetchPerson(int ID)
        {
            return Person.Load(ID);
        }

        [DataObjectMethod(DataObjectMethodType.Insert)]
        public Person CreatePerson(Person obj)
        {
            obj.Save();
            return obj;
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public void UpdatePerson(Person obj)
        {
            Person.Evict(obj.ID);
            obj.Save();
        }

        [DataObjectMethod(DataObjectMethodType.Delete)]
        public void DeletePerson(Person obj)
        {
            if (SearchAddresses(obj.ID).Count > 0)
                throw new DataException("Remove the child addresses before deleting the person");

            Person.Evict(obj.ID);
            obj.Delete();
            obj.Save();
        }

        #endregion

        #region Address Members

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<Address> SearchAddresses(int personID)
        {
            return Address.Search(QrySearchAddressesByContact.Query(personID));
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public Address FetchAddress(int ID)
        {
            return Address.Load(ID);
        }

        [DataObjectMethod(DataObjectMethodType.Insert)]
        public Address CreateAddress(Address obj)
        {
            obj.Save();
            return obj;
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public void UpdateAddress(Address obj)
        {
            obj.Save();
        }

        [DataObjectMethod(DataObjectMethodType.Delete)]
        public void DeleteAddress(Address obj)
        {
            Address.Evict(obj.ID);
            obj.Delete();
            obj.Save();
        }

        #endregion
    }
}
