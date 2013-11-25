using System;

namespace NHibernate.Carbon.Tests.Domain.OnlineShopping
{
	public class Product
	{
		private string _description = string.Empty;
		private int _id = 0;
		private string _name = string.Empty;
		private decimal? _price = 0.0M;

		public Product()
		{
		}

		//public Product(OrderLine orderLine)
		//{
		//    OrderLine = orderLine;
		//}

		public virtual int Id
		{
			get { return _id; }
		}

		private int _version;
		public virtual int Version
		{
			get { return _version; }
			set { _version = value; }
		}

		private DateTime? _createdon;
		public virtual DateTime? CreatedOn
		{
			get { return _createdon; }
			set { _createdon = value; }
		}

		public virtual string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual decimal? Price
		{
			get { return _price; }
			set { _price = value; }
		}

		//private OrderLine _orderline;
		//public virtual OrderLine OrderLine
		//{
		//    get { return _orderline; }
		//    private set { _orderline = value; }
		//}


		public virtual void ChangeProduct(string name, string description, decimal price)
		{
			Name = name;
			Description = description;
			Price = price;
		}
	}
}