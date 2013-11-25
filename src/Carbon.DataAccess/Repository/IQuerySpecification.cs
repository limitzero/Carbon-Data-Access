using System.Collections.Generic;

namespace NHibernate.Carbon.Repository
{
    /// <summary>
    /// Contract for implementing in-memory querying against a collection of items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQuerySpecification<T> where T : class
    {
        /// <summary>
        /// (Write-Only).  Current session via NHibernate to access data store.
        /// </summary>
        ISession Session { set;  }

        /// <summary>
        /// (Read-Write). The in-memory collection of data to be filtered.
        /// </summary>
        IList<T> Source { set; get; }

        /// <summary>
        /// This will find the listing of items based on custom filter applied to the <seealso cref="Session">NHibernate session</seealso>
        /// or <seealso cref="Source">the custom in-memory data collection</seealso>.
        /// </summary>
        IList<T> Find();

    }
}