using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.DataAccess.Tests.Domain
{
    public enum OrderPriorityEnums
    {
        NextDayAir, 
        RegularGround, 
        NextSecond
    }

    public class OrderPriority
    {
        private int _id = 0;
        private OrderPriorityEnums _priority; 

        // for NHibernate
        protected OrderPriority()
        {}

        public OrderPriority(OrderPriorityEnums priority)
        {
            _priority = priority;
        }

        public virtual int Id
        {
            get { return _id; }
        }

        public virtual OrderPriorityEnums Priority 
        {
            get {return _priority;}
        }

    }
}