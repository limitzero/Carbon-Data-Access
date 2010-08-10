using System.Collections.Generic;

namespace Carbon.Repository.Repository
{
    /// <summary>
    /// Actions that can be applied to the entity for persistance to the data store.
    /// </summary>
    public enum PersistanceAction
    {
        /// <summary>
        /// Option to save the entity.
        /// </summary>
        Save,

        /// <summary>
        /// Option to delete the entity.
        /// </summary>
        Delete
    }

    /// <summary>
    /// Contract for basic operations for data retrieval and persistance based on type.
    /// </summary>
    /// <typeparam name="T">Type to use for data retreival and persistance against the data store.</typeparam>
    public interface IRepository<T> 
        where T : class 
    {
        /// <summary>
        /// This will find a single instance of a type from the object identifier.
        /// </summary>
        /// <param name="id">Identifier to use to select a single instance</param>
        /// <returns></returns>
        T FindById(object id);

        /// <summary>
        /// This will find a single instance of a type from the <seealso cref="IQuerySpecification{T}">query specification</seealso>.
        /// </summary>
        /// <param name="specification">The query specification to use for filtering data.</typeparam>
        /// <returns></returns>
        T FindOne(AbstractQuerySpecification<T> specification);

        /// <summary>
        /// This will return the entire listing of entities based on type.
        /// </summary>
        /// <returns></returns>
        IList<T> FindAll();

        /// <summary>
        /// This will return the entire listing of entities based on type with a filter applied.
        /// </summary>
        /// <param name="specification">The query specification to use for filtering data.</typeparam>
        /// <returns></returns>
        IList<T> FindAll(AbstractQuerySpecification<T> specification);

        /// <summary>
        /// This will return a paginated result based on the list passed.
        /// </summary>
        /// <param name="source">Enumerable list of entities to apply pagination to.</param>
        /// <param name="page">Current page number</param>
        /// <param name="pageSize">Number of records to return/display at one time.</param>
        /// <returns></returns>
        PaginatedResult<T> FindAllWithPagination(IList<T> source, int page, int pageSize);

        /// <summary>
        /// This will return a paginated result based on the list of items defined for the repository.
        /// </summary>
        /// <param name="page">Current page number</param>
        /// <param name="pageSize">Number of records to return/display at one time.</param>
        /// <returns></returns>
        PaginatedResult<T> FindAllWithPagination(int page, int pageSize);

        /// <summary>
        /// This will return a paginated result based on the query passed.
        /// </summary>
        /// <param name="specification">The query specification to use for filtering data.</typeparam>
        /// <param name="page">Current page number</param>
        /// <param name="pageSize">Number of records to return/display at one time.</param>
        /// <returns></returns>
        PaginatedResult<T> FindAllWithPaginationAndSpecification(AbstractQuerySpecification<T> specification,   int page, int pageSize);
 
        /// <summary>
        /// This will persist the entity to the data store based on the <seealso cref="PersistanceAction">persistance action.</seealso>
        /// </summary>
        /// <param name="action">Persistance action to perform on the entity</param>
        /// <param name="entity">Entity to persist to the data store.</param>
        void Persist(PersistanceAction action, T entity);
    }
}