using System.Collections.Generic;

namespace NHibernate.Carbon.Tests.Domains.OnlineCart.Model
{
	public class Order
	{
		private int _id = 0;
		public virtual int Id
		{
			get { return _id; }
			private set { _id = value; }
		}

		private Customer _customer;
		public virtual Customer Customer
		{
			get { return _customer; }
			private set { _customer = value; }
		}

		private IList<OrderLine> _orderlines = new List<OrderLine>();
		public virtual IList<OrderLine> OrderLines
		{
			get { return _orderlines; }
			private set { _orderlines = value; }
		}

		// needed for NHibernate:
		protected Order()
		{
		
		}

		public Order(Customer customer)
		{
			_customer = customer;
		}

		public virtual void CreateOrderLine(Product product, int quantity)
		{
			var orderline = new OrderLine(this);
			orderline.RecordItem(product, quantity);
		}
	}
}
