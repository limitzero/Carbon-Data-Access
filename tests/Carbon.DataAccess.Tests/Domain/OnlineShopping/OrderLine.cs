namespace NHibernate.Carbon.Tests.Domain.OnlineShopping
{
	public class OrderLine
	{
		private int _id = 0;

		protected OrderLine()
		{
		}

		public OrderLine(Order order)
		{
			Order = order;
		}

		public virtual int Id
		{
			get { return _id; }
		}

		private Product _product;

		/// <summary>
		/// Read-Only. Instance of the product that should be attached to the order line.
		/// </summary>
		public virtual Product Product
		{
			get { return _product; }
			set { _product = value; }
		}

		private int _quantity;
		public virtual int Quantity
		{
			get { return _quantity; }
			set { _quantity = value; }
		}

		private Order _order;
		public virtual Order Order
		{
			get { return _order; }
			private set { _order = value; }
		}

		public virtual Product CreateProduct()
		{
			// Note: broke the domain model here for product belonging on order line:
			return new Product();
		}

		public virtual void SaveProduct(Product product, int quantity)
		{
			Product = product;
			Quantity = quantity;
		}
	}
}