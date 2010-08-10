using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.DataAccess.Tests.Domain
{
    public enum OrderStatusEnum
    {
        Created,
        OnHold, 
        Completed
    }

    public class OrderStatus
    {
        private int _id = 0;
        private OrderStatusEnum _status;

        // for NHibernate:
        private OrderStatus()
        { }

        public OrderStatus(OrderStatusEnum status)
        {
            _status = status;
        }

        public int Id
        {
            get { return _id; }
        }

        public OrderStatusEnum Status
        {
            get { return _status; }
        }

    }
}