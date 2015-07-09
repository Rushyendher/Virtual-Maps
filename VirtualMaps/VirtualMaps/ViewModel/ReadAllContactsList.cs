using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualMaps.Model;
using System.Collections.ObjectModel;

namespace VirtualMaps.ViewModel
{
    public class ReadAllContactsList
    {
        DatabaseHelperClass Db_Helper = new DatabaseHelperClass();
        public ObservableCollection<Contacts> GetAllContacts()
        {
            return Db_Helper.ReadContacts();
        }
    }
}
