using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.DataAccess.Tests.Domain
{
    public class Order
    {
        private IList<OrderLine> _orderlines = new List<OrderLine>();
        private int _id;
        private Address _homeaddress;
        private ShippingAddress _shippingaddress;
        private DateTime _creationdate = System.DateTime.Now;
        private OrderStatus _status;
        private OrderPriority _priority;

        protected Order()
        {}

        public Order(OrderStatus status, OrderPriority priority)
        {
            _status = status;
            _priority = priority;
        }

        public virtual int Id
        {
            get
            {
                return _id;
            }

        }

        public virtual IList<OrderLine> Orderlines
        {
            get
            {
                return _orderlines;
            }
        }

        /// <summary>
        /// (Read-Only)
        /// </summary>
        public virtual Address HomeAddress
        {
            get
            {
                return _homeaddress;
            }
            set
            {
                _homeaddress = value;
            }
        }

        public virtual ShippingAddress ShippingAddress
        {
            get
            {
                return _shippingaddress;
            }
            set
            {
                _shippingaddress = value;
            }
        }

        /// <summary>
        /// (Read-Only)
        /// </summary>
        public virtual DateTime CreationDate
        {
            get
            {
                return _creationdate;
            }
         
        }

        public virtual OrderStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public virtual OrderPriority Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }

        }

        public  virtual void AddItem(OrderLine lineItem)
        {
            if (!_orderlines.Contains(lineItem))
                _orderlines.Add(lineItem);
        }
    }
}