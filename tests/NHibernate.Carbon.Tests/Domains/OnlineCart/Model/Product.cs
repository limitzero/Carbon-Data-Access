namespace NHibernate.Carbon.Tests.Domains.OnlineCart.Model
{
	public class Product
	{
		private int _id = 0;
		public virtual int Id
		{
			get { return _id; }
			private set { _id = value; }
		}



		private string _sku;
		public virtual string SKU
		{
			get { return _sku; }
			private set { _sku = value; }
		}

		private string _model;
		public virtual string Model
		{
			get { return _model; }
			private set { _model = value; }
		}

		private string _name;
		public virtual string Name
		{
			get { return _name; }
			private set { _name = value; }
		}

		private string _description;
		public virtual string Description
		{
			get { return _description; }
			private set { _description = value; }
		}

		private decimal _price;
		public virtual decimal Price
		{
			get { return _price; }
			private set { _price = value; }
		}

		protected Product()
		{
			
		}

		public Product(string sku, string name, string model, string description)
		{
			this._sku = sku;
			this._name = name;
			this._model = model;
			this._description = description;
		}

		public void ChangePrice(decimal price)
		{
			this.Price = price;
		}
	}
}
