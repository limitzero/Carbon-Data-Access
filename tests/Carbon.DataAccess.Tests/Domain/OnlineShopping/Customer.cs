using System;
using System.Collections.Generic;

namespace NHibernate.Carbon.Tests.Domain.OnlineShopping
{
	public class Customer
	{
		private int _id = 0;
		public virtual int Id
		{
			get { return _id; }
		}

		// name is a component of 'Customer'
		private Name _name;
		public virtual Name Name
		{
			get { return _name; }
		}

		private IList<Order> _orders = new List<Order>();
		public virtual IList<Order> Orders
		{
			get { return _orders; }
			private set { _orders = value; }
		}

		public virtual void ChangeName(Name name)
		{
			_name = name;
		}
	}
}