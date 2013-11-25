namespace NHibernate.Carbon.Tests.Domains.OnlineCart.Model
{
	public class BillingAddress : Address
	{
		private int _id;
		public virtual int Id
		{
			get { return _id; }
			private set { _id = value; }
		}

		protected BillingAddress()
		{
		}

		public BillingAddress(string address1, string address2, string city, string state, string postalCode)
			:base(address1,  address2,  city,  state,  postalCode)
		{
		
		}


	}
}