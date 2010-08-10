using System.Collections.Generic;
using NHibernate;

namespace Carbon.Repository.Repository
{
    public abstract class AbstractQuerySpecification<T> : IQuerySpecification<T> where T : class
    {
        public ISession Session { set; private get; }

        public IList<T> Source { set;  get; }

        public IList<T> Find()
        {
            IList<T> results = new List<T>();

            if (Source != null)
                results = this.FilterInMemory();
            else
            {
                results = this.FilterViaCriteria();
            }

            return results;
        }

        /// <summary>
        /// This will return the current session for access.
        /// </summary>
        /// <returns></returns>
        public ISession GetSession()
        {
            return this.Session;
        }

        /// <summary>
        /// This will perform the in-memory filtering or search on the data based on LINQ or other custom methods.
        /// </summary>
        /// <returns></returns>
        public virtual IList<T> FilterInMemory()
        {
            return null;
        }

        /// <summary>
        /// This will perform a filtering or search on the data based on the criteria object.
        /// </summary>
        /// <returns></returns>
        public virtual IList<T> FilterViaCriteria()
        {
            return null;
        }

    }
}