using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace VirtualMaps.Model
{
    public class Contacts : INotifyPropertyChanged
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        private string idvalue;
        private string NameValue = String.Empty;
        private string EmailValue = String.Empty;
        private string PhoneNumberValue = String.Empty;
        private string LocationValue = String.Empty;

        public string Name 
        {
            get { return this.NameValue; }
            set
            {
                if (value != this.NameValue)
                {
                    this.NameValue = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string Email
        {
            get { return this.EmailValue; }
            set
            {
                if (value != this.EmailValue)
                {
                    this.EmailValue = value;
                    NotifyPropertyChanged("Email");
                }
            }
        }

        public string PhoneNumber
        {
            get { return this.PhoneNumberValue; }
            set
            {
                if (value != this.PhoneNumberValue)
                {
                    this.PhoneNumberValue = value;
                    NotifyPropertyChanged("PhoneNumber");
                }
            }
        }

        public string Location
        {
            get { return this.LocationValue; }
            set
            {
                if (value != this.LocationValue)
                {
                    this.LocationValue = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }


        public Contacts()
        { 
        
        }

        public Contacts(string name, string email, string phone_no, string loc)
        {
            Name = name;
            Email = email;
            PhoneNumber = phone_no;
            Location = loc;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
