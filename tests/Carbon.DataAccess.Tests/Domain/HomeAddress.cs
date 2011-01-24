using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.DataAccess.Tests.Domain
{
    /// <summary>
    /// HomeAddress "is a" Address, but the entity that should 
    /// be preserved is the the HomeAddress, not the base value type
    /// this is why it needs an identifier column.
    /// </summary>
    public class HomeAddress  :  Address
    {
        private int _id = 0;
        private int _yearsatresidence = 0;

        public virtual int Id
        {
            get
            {
                return _id;
            }
        }

        public virtual int YearsAtResidence
        {
            get { return _yearsatresidence; }
            set { _yearsatresidence = value; }
        }

    }
}