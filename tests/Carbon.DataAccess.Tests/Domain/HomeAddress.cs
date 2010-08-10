using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.DataAccess.Tests.Domain
{
    public class HomeAddress  :  Address
    {
        private int _yearsatresidence = 0;

        private HomeAddress()
        { }

        public int YearsAtResidence
        {
            get { return _yearsatresidence; }
            set { _yearsatresidence = value;}
        }

    }

    public class ShippingAddress : Address
    {
        private ShippingAddress()
        { }
    }
}