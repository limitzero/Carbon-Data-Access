namespace NHibernate.Carbon.Tests.Domain.OnlineShopping
{
	public abstract class Address
	{
		private string _address1 = string.Empty;
		public virtual string Address1
		{
			get { return _address1; }
			set { _address1 = value; }
		}

		private string _address2 = string.Empty;
		public virtual string Address2
		{
			get { return _address2; }
			set { _address2 = value; }
		}

		private string _city = string.Empty;
		public virtual string City
		{
			get { return _city = string.Empty; }
			set { _city = value; }
		}

		private string _state = string.Empty;
		public virtual string State
		{
			get { return _state; }
			set { _state = value; }
		}

		private string _postalcode = string.Empty;
		public virtual string PostalCode
		{
			get { return _postalcode; }
			set { _postalcode = value; }
		}

		public virtual void ChangeAddress(string addressLine1, string addressLine2, string city, string state,
		                                  string postalCode)
		{
			Address1 = addressLine1;
			Address2 = addressLine2;
			City = city;
			State = state;
			PostalCode = postalCode;
		}
	}
}