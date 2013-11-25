using System;
using System.Collections.Generic;

namespace NHibernate.Carbon.Tests.Domain.OnlineShopping
{
	public class Order
	{
		private readonly DateTime _creationdate = DateTime.Now;
		private readonly IList<OrderLine> _orderlines = new List<OrderLine>();
		private HomeAddress _homeaddress;
		private int _id = 0;
		private OrderPriority _priority;
		private ShippingAddress _shippingaddress;
		private OrderStatus _status;

		// needed for NHibernate
		protected Order()
		{
		}

		public Order(OrderStatus status, OrderPriority priority)
			: this(status, priority, null, null)
		{
		}

		public Order(OrderStatus status,
		             OrderPriority priority,
		             HomeAddress homeAddress = null,
		             ShippingAddress shippingAddress = null)
		{
			_status = status;
			_priority = priority;
			_homeaddress = homeAddress;
			_shippingaddress = shippingAddress;
			_creationdate = System.DateTime.UtcNow;
		}

		public virtual int Id
		{
			get { return _id; }
		}

		public virtual IList<OrderLine> Orderlines
		{
			get { return _orderlines; }
		}

		/// <summary>
		/// (Read-Only)
		/// </summary>
		public virtual HomeAddress HomeAddress
		{
			get { return _homeaddress; }
			private set { _homeaddress = value; }
		}

		public virtual ShippingAddress ShippingAddress
		{
			get { return _shippingaddress; }
			private set { _shippingaddress = value; }
		}

		/// <summary>
		/// (Read-Only)
		/// </summary>
		public virtual DateTime CreationDate
		{
			get { return _creationdate; }
		}

		public virtual OrderStatus Status
		{
			get { return _status; }
			private set { _status = value; }
		}

		public virtual OrderPriority Priority
		{
			get { return _priority; }
			private set { _priority = value; }
		}

		public virtual OrderLine CreateOrderLine()
		{
			return new OrderLine(this);
		}

		public virtual void AddLine(OrderLine lineItem)
		{
			if (!_orderlines.Contains(lineItem))
				_orderlines.Add(lineItem);
		}

		public virtual void ChangeHomeAddress(string addressLine1, string addressLine2,
		                                      string city, string state, string postalCode)
		{
			if (HomeAddress == null)
				HomeAddress = new HomeAddress();

			HomeAddress.ChangeAddress(addressLine1, addressLine2, city, state, postalCode);
		}

		public virtual void ChangeShippingAddress(string addressLine1, string addressLine2,
		                                          string city, string state, string postalCode)
		{
			if (ShippingAddress == null)
				ShippingAddress = new ShippingAddress();

			ShippingAddress.ChangeAddress(addressLine1, addressLine2, city, state, postalCode);
		}
	}
}