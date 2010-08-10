using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.DataAccess.Tests.Domain
{
    public class Product
    {
        private string _name = string.Empty;
        private string _description = string.Empty;
        private decimal _price =0.0M;
        private int _id = 0;

        protected Product()
        {

        }

        public Product(string name, string description, decimal price)
        {
            _name = name;
            _description = description;
            _price = price;
        }



        public virtual string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public virtual string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public virtual decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
            }
        }

        public virtual int Id
        {
            get
            {
                return _id;
            }

        }
    }
}