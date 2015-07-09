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
using Microsoft.Phone.Tasks;

namespace VirtualMaps
{
    public partial class ContactInfo : PhoneApplicationPage
    {
        string name_of_the_person;

        DatabaseHelperClass Db_Helper = new DatabaseHelperClass();
        Contacts currentcontact = new Contacts();
        Contacts id = new Contacts();

        string phn_Number;
        public ContactInfo()
        {
            InitializeComponent();
            this.Loaded += ContactInfo_Loaded;
        }

        void ContactInfo_Loaded(object sender, RoutedEventArgs e)
        {
          
            nameofperson.Text = name_of_the_person;
            GetDetails();
        }

        private void GetDetails()
        {
            currentcontact = Db_Helper.ReadContact(2);
            email.Text = currentcontact.Email;
            num.Text = currentcontact.PhoneNumber;
            phn_Number = num.Text;
            loc.Text = currentcontact.Location;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string localvalue;
            if(NavigationContext.QueryString.TryGetValue("parameter",out localvalue))
                name_of_the_person = localvalue;
        }

        private void call_Click(object sender, EventArgs e)
        {
            PhoneCallTask phntask = new PhoneCallTask();
            phntask.PhoneNumber = phn_Number;
            phntask.DisplayName = name_of_the_person;
            phntask.Show();
        }

        private void msg_Click(object sender, EventArgs e)
        {
            SmsComposeTask smsComposeTask = new SmsComposeTask();
            smsComposeTask.To = phn_Number;
            smsComposeTask.Show();
        }
    }
}