using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.DataAccess.Tests.Domain
{
    public class OrderLine
    {
        private Product _product;
        private int _quantity = 0;
        private int _id = 0;

        protected OrderLine()
        { }

        public OrderLine(Product product, int quantity)
        {
            _product = product;
            _quantity = quantity;
        }

        public virtual int Id
        {
            get
            {
                return _id;
            }

        }

        /// <summary>
        /// Read-Only. Instance of the product that should be attached to the order line.
        /// </summary>
        public virtual Product Product
        {
            get
            {
                return _product;
            }
            set
            {
                _product = value;
            }
        }

        public virtual int Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
            }
        }

        public virtual void ChangeProductTo(Product newProduct)
        {
            _product = newProduct;
        }

    }
}