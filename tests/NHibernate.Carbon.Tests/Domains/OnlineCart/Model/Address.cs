namespace NHibernate.Carbon.Tests.Domains.OnlineCart.Model
{
	public abstract class Address
	{
		private string _address1;
		public virtual string Address1
		{
			get { return _address1; }
			private set { _address1 = value; }
		}

		private string _address2;
		public virtual string Address2
		{
			get { return _address2; }
			private set { _address2 = value; }
		}

		private string _city;
		public virtual string City
		{
			get { return _city; }
			private set { _city = value; }
		}

		private string _state;
		public virtual string State
		{
			get { return _state; }
			private set { _state = value; }
		}

		private string _postalcode;
		public virtual string PostalCode
		{
			get { return _postalcode; }
			private set { _postalcode = value; }
		}

		protected Address()
		{
			
		}

		public Address(string address1, string address2, string city, string state, string postalcode)
		{
			_address1 = address1;
			_address2 = address2;
			_city = city;
			_state = state;
			_postalcode = postalcode;
		}

		public virtual void Change(string address1, string address2, string city, string state, string postalcode)
		{
			_address1 = address1;
			_address2 = address2;
			_city = city;
			_state = state;
			_postalcode = postalcode;
		}
	}
}