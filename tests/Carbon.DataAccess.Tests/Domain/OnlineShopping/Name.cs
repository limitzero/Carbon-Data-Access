namespace NHibernate.Carbon.Tests.Domain.OnlineShopping
{
	public class Name
	{
		private string _first;
		private string _last;
		private string _middle;
		private string _suffix;

		private Name()
		{
		}

		public Name(string first, string last)
			: this(first, last, string.Empty, string.Empty)
		{
		}

		public Name(string first, string last, string middle, string suffix)
		{
			_first = first;
			_last = last;
			_middle = middle;
			_suffix = suffix;
		}

		public virtual string First
		{
			get { return _first; }
			private set { _first = value; }
		}

		public virtual string Last
		{
			get { return _last; }
			private set { _last = value; }
		}

		public virtual string Middle
		{
			get { return _middle; }
			private set { _middle = value; }
		}

		public virtual string Suffix
		{
			get { return _suffix; }
			private set { _suffix = value; }
		}
	}
}