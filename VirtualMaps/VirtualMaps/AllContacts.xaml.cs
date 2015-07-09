using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VirtualMaps.Model;
using VirtualMaps.ViewModel;
using System.Collections.ObjectModel;
using SQLite;

namespace VirtualMaps
{
    public partial class AllContacts : PhoneApplicationPage
    {
        ObservableCollection<Contacts> DB_ContactList = new ObservableCollection<Contacts>();

        public AllContacts()
        {
            InitializeComponent();
            this.Loaded += AllContacts_Loaded;
        }

        private void AllContacts_Loaded(object sender, RoutedEventArgs e)
        {
            ReadAllContactsList dbcontacts = new ReadAllContactsList();
            DB_ContactList = dbcontacts.GetAllContacts();
            lst_contacts.ItemsSource = DB_ContactList.OrderByDescending(i => i.Id).ToList();
        }

        private void dlt_all_Click(object sender, EventArgs e)
        {
            DatabaseHelperClass Db_Helper = new DatabaseHelperClass();
            Db_Helper.DeleteAllContact();//delete all DB contacts
            DB_ContactList.Clear();//Clear collections
            lst_contacts.ItemsSource = DB_ContactList;
        }

    }
}