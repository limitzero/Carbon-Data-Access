using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.DataAccess.Tests.Domain
{
    public  class Address
    {
        private int _id = 0;
        private string _address1;
        private string _address2;
        private string _city;
        private string _state;
        private string _postalcode;

        public int Id
        {
            get
            {
                return _id;
            }

        }

        public string Address1
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public string Address2
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public string City
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public string State
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public string PostalCode
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}