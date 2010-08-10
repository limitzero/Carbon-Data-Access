using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Carbon.Repository.Repository
{
    /// <summary>
    /// Concrete implemenation of a repository based on NHibernate
    /// </summary>
    /// <typeparam name="T">Type to use for data retrieval and persistance</typeparam>
    public class NHibernateRepository<T> : IRepository<T> where T : class
    {
        private readonly ISession _session;

        public NHibernateRepository(ISession session)
        {
            _session = session;
        }

        public T FindById(object id)
        {
            return _session.Get<T>(id);
        }

        public T FindOne(AbstractQuerySpecification<T> specification)
        {
            var retval = default(T);

            PrepareSpecification(specification);

            var listing = specification.Find();

            if (listing.Count > 0)
                retval = listing[0];

            return retval;
        }

        public IList<T> FindAll()
        {
            var criteria = DetachedCriteria.For<T>()
                .SetResultTransformer(new DistinctRootEntityResultTransformer());

            var results = criteria.GetExecutableCriteria(_session).List<T>();

            return results;
        }

        public IList<T> FindAll(AbstractQuerySpecification<T> specification)
        {
            PrepareSpecification(specification);
            return specification.Find();
        }

        public PaginatedResult<T> FindAllWithPagination(int page, int pageSize)
        {
            var source = this.FindAll();
            var pr = new PaginatedResult<T>(source, page, pageSize);
            return pr;
        }

        public PaginatedResult<T> FindAllWithPagination(IList<T> source, int page, int pageSize)
        {
            var pr = new PaginatedResult<T>(source, page, pageSize);
            return pr;
        }


        public PaginatedResult<T> FindAllWithPaginationAndSpecification(AbstractQuerySpecification<T> specification, int page, int pageSize)
        {
            PrepareSpecification(specification);
            var listing = specification.Find();

            var pr = new PaginatedResult<T>(listing, page, pageSize);
            return pr;
        }

        public void Persist(PersistanceAction action, T item)
        {
            if (action == PersistanceAction.Save)
                RunInTransaction(() => _session.SaveOrUpdate(item));

            if (action == PersistanceAction.Delete)
                RunInTransaction(() => _session.Delete(item));
        }

        private void RunInTransaction(Action action)
        {
            using (var tx = _session.BeginTransaction())
            {
                try
                {
                    action.Invoke();
                    tx.Commit();
                }
                catch (Exception exception)
                {
                    tx.Rollback();
                    throw;
                }

            }
        }

        private void PrepareSpecification(AbstractQuerySpecification<T> specification)
        {
            PrepareSpecification(specification, null);
        }

        private void PrepareSpecification(AbstractQuerySpecification<T> specification, IList<T> source)
        {
            specification.Session = _session;
            if(specification.Source == null)
                specification.Source = source;
        }
    }
}