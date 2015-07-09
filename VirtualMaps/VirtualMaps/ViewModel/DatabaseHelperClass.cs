using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualMaps.Model;
using SQLite;
using System.Collections.ObjectModel;

namespace VirtualMaps.ViewModel
{
    public class DatabaseHelperClass
    {
        SQLiteConnection dbConn;

        public int total_rows;

        public async Task<bool> onCreate(string DB_PATH)
        {
            try
            {
                if (!CheckFileExists(DB_PATH).Result)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<Contacts>();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Contacts ReadContact(int contactid)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingconact = dbConn.Query<Contacts>("select * from Contacts where Id =" + contactid).FirstOrDefault();
                return existingconact;
            }
        }

        public int TotalRows()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var allrows = dbConn.Query<Contacts>("select Id from Contacts");
                var count = allrows.Any() ? allrows.Count : 0;
                return count;
            }
        }



        public ObservableCollection<Contacts> ReadContacts()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Contacts> myCollection = dbConn.Table<Contacts>().ToList<Contacts>();
                ObservableCollection<Contacts> ContactsList = new ObservableCollection<Contacts>(myCollection);
                return ContactsList;
            }
        }

        public void UpdateContact(Contacts contact)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingconact = dbConn.Query<Contacts>("select * from Contacts where Id =" + contact.Id).FirstOrDefault();
                if (existingconact != null)
                {
                    existingconact.Name = contact.Name;
                    existingconact.Email = contact.Email;
                    existingconact.PhoneNumber = contact.PhoneNumber;
                    existingconact.Location = contact.Location;
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(existingconact);
                    });
                }
            }   
        }


        public void Insert(Contacts newcontact)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(newcontact);
                });
            }
        }

        public void DeleteContact(int Id)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingconact = dbConn.Query<Contacts>("select * from Contacts where Id =" + Id).FirstOrDefault();
                if (existingconact != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(existingconact);
                    });
                }
            }
        }

        public void DeleteAllContact()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                //dbConn.RunInTransaction(() =>
                //   {
                dbConn.DropTable<Contacts>();
                dbConn.CreateTable<Contacts>();
                dbConn.Dispose();
                dbConn.Close();
                //});
            }
        }
    }
}
