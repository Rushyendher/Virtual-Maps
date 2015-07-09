using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VirtualMaps.ViewModel;
using VirtualMaps.Model;
using SQLite;

namespace VirtualMaps
{
    public partial class SavePhoneNumber : PhoneApplicationPage
    {
        public SavePhoneNumber()
        {
            InitializeComponent();
        }

        private void ResetAll()
        {
            txt_name.Text = "";
            txt_email.Text = "";
            txt_number.Text = "";
            txt_location.Text = "";
        }

        private async void save_Click(object sender, EventArgs e)
        {
            DatabaseHelperClass Db_helper = new DatabaseHelperClass();
            if (txt_name.Text != "" & txt_email.Text != "" & txt_number.Text != "" & txt_location.Text != "")
            { 
                Db_helper.Insert(new Contacts(txt_name.Text,txt_email.Text,txt_number.Text,txt_location.Text));
                MessageBox.Show("Contact saved", "Success :)", MessageBoxButton.OK);
                ResetAll();
            }
            else
            {
                MessageBox.Show("Fill all the fields","Error :(",MessageBoxButton.OK);
            }
        }

        private void shwcntacts_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AllContacts.xaml",UriKind.RelativeOrAbsolute));
        }
    }
}