namespace NHibernate.Carbon.Tests.Domains.OnlineCart.Model
{
	// component class:
	public class Name
	{
		private string _first;
		public virtual string First
		{
			get { return _first; }
			private set { _first = value; }
		}

		private string _middle;
		public virtual string Middle
		{
			get { return _middle; }
			private set { _middle = value; }
		}

		private string _last;
		public virtual string Last
		{
			get { return _last; }
			private set { _last = value; }
		}

		protected Name()
		{
		}

		public Name(string first, string last)
			:this(first, string.Empty, last)
		{
		}

		public Name(string first, string middle, string last)
		{
			_first = first;
			_middle = middle;
			_last = last;
		}
	}
}