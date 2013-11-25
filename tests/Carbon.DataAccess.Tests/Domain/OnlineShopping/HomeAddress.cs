namespace NHibernate.Carbon.Tests.Domain.OnlineShopping
{
	/// <summary>
	/// HomeAddress "is a" Address, but the entity that should 
	/// be preserved is the the HomeAddress, not the base value type
	/// this is why it needs an identifier column. This situation changes 
	/// the HomeAddress from just being metadata on the parent object 
	/// to an actual persisted entity with a relationship to the parent (i.e.  Orders).
	/// </summary>
	public class HomeAddress : Address
	{
		private int _id = 0;

		public virtual int Id
		{
			get { return _id; }
		}

		private int _yearsatresidence;
		public virtual int YearsAtResidence
		{
			get { return _yearsatresidence; }
			set { _yearsatresidence = value; }
		}
	}
}