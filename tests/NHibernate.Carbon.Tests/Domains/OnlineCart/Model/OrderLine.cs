namespace NHibernate.Carbon.Tests.Domains.OnlineCart.Model
{
	public class OrderLine
	{
		private int _id = 0;
		public virtual int Id
		{
			get { return _id; }
			private set { _id = value; }
		}

		private Order _order;

		public virtual Order Order
		{
			get { return _order; }
			private set { _order = value; }
		}


		private Product _product;

		/// <summary>
		/// </summary>
		public virtual Product Product
		{
			get { return _product; }
			private set { _product = value; }
		}

		private int _quantity;
		public virtual int Quantity
		{
			get { return _quantity; }
			private set { _quantity = value; }
		}

		protected OrderLine()
		{

		}

		public OrderLine(Order order)
		{
			this.Order = order;
		}

		public virtual void RecordItem(Product product, int quantity)
		{
			this.Product = product;
			this.Quantity = quantity;
		}


	}
}
