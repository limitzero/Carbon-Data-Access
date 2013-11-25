using System;

namespace NHibernate.Carbon.Tests.Domains.OnlineBlog.Model
{
	public abstract class Entity<TEntity> 
		where TEntity : class 
	{
		private Guid _id;

		/// <summary>
		/// Gets or sets the instance identifier of the entity
		/// </summary>
		public virtual Guid Id
		{
			get { return _id; }
			private set { _id = value; }
		}

		public override bool Equals(object obj)
		{
			var other = obj as Entity<TEntity>;
			if (other == null) return false;
			var thisIsNew = Equals(Id, Guid.Empty);
			var otherIsNew = Equals(other.Id, Guid.Empty);
			if (thisIsNew && otherIsNew)
				return ReferenceEquals(this, other);
			return Id.Equals(other.Id);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = Id.GetHashCode();
				result = (result*397);
				return result;
			}
		}

		public static bool operator == (Entity<TEntity> lhs, Entity<TEntity> rhs)
		{
			return Equals(lhs, rhs);
		}

		public static bool operator !=(Entity<TEntity> lhs, Entity<TEntity> rhs)
		{
			return !Equals(lhs, rhs);
		}
	}
}