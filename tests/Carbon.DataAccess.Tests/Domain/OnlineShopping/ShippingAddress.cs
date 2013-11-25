namespace NHibernate.Carbon.Tests.Domain.OnlineShopping
{
	/// <summary>
	/// ShippingAddress "is a" Address, but the entity that should 
	/// be preserved is the the ShippingAddress, not the base value type
	/// this is why it needs an identifier column.
	/// </summary>
	public class ShippingAddress : Address
	{
		private int _id = 0;
		public virtual int Id
		{
			get { return _id; }
		}
	}
}