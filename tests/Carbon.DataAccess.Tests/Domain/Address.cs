using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.DataAccess.Tests.Domain
{
    public abstract class Address
    {
        private string _address1;
        private string _address2;
        private string _city;
        private string _state;
        private string _postalcode;

        public virtual string Address1
        {
            get { return _address1; }
            set { _address2 = value; }
        }

        public virtual string Address2
        {
            get { return _address2; }
            set { _address2 = value; }
        }

        public virtual string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public virtual string State
        {
            get { return _state; }
            set { _state = value; }
        }

        public virtual string PostalCode
        {
            get { return _postalcode; }
            set { _postalcode = value; }
        }

        public virtual void ChangeAddress(string addressLine1, string addressLine2, string city, string state, string postalCode)
        {
            this.Address1 = addressLine1;
            this.Address2 = addressLine2;
            this.City = city;
            this.State = state;
            this.PostalCode = postalCode;
        }
    }
}