namespace NHibernate.Carbon.Tests.Domain.OnlineShopping
{
	public enum OrderStatusEnum
	{
		Created,
		OnHold,
		Completed
	}

	public class OrderStatus
	{
		private readonly OrderStatusEnum _status;
		private int _id = 0;

		// for NHibernate:
		protected OrderStatus()
		{
		}

		public OrderStatus(OrderStatusEnum status)
		{
			_status = status;
		}

		public virtual int Id
		{
			get { return _id; }
		}

		public virtual OrderStatusEnum Status
		{
			get { return _status; }
		}
	}
}