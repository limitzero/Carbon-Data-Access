namespace NHibernate.Carbon.Tests.Domain.OnlineShopping
{
	public enum OrderPriorityEnums
	{
		NextDayAir,
		RegularGround,
		NextSecond
	}

	public class OrderPriority
	{
		private readonly OrderPriorityEnums _priority;
		private int _id = 0;

		// for NHibernate
		protected OrderPriority()
		{
		}

		public OrderPriority(OrderPriorityEnums priority)
		{
			_priority = priority;
		}

		public virtual int Id
		{
			get { return _id; }
		}

		public virtual OrderPriorityEnums Priority
		{
			get { return _priority; }
		}
	}
}