using System.Linq.Expressions;

namespace Demo.Interfaces
{
    public interface IRepository<T> where T : class
    {

        // Get object by its key id
        T GetById(int id);

        // Used to get record of a single object (similiar to a SQL SELECT...WHERE ID =)
        // A Func<T, bool> represents a function that takes an object of type T and returns a bool.
        // It's commonly referred to as a predicate, and is used to verify a condition on an object.
        // Expression<Func<T>> is a description of a function as an expression tree.
        // It can be compiled at run time that generates a Func<T>
        // It can also be translated to other languages e.g. SQL in LINQ to SQL.
        // NotTracking is ReadOnly Results
        // Includes used for Joining of other objects.
        T Get(Expression<Func<T, bool>> predicate, bool asNotTracking = false, string includes = null);

        //Same as above, but Asynchronous call
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool asNotTracking = false, string includes = null);

        //Same to Get, but expects to return move than one record
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        // Returns an Enumerable list of object results to iterate through
        IEnumerable<T> List();

        // Returns an Enumerable list of object results to iterate through.  Adds Where Expressions, OrderBy, and Joins
        IEnumerable<T> List(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> orderBy = null, string includes = null);

        // Same as above by Asynchronous action
        Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> orderBy = null, string includes = null);

        // Add (Inserts) a new record instance
        void Add(T entity);

        // Delete (Removes) a single record instance
        void Delete(T entity);

        // Delete (Removes) multiple record instances
        void Delete(IEnumerable<T> entities);

        // Updates all changes to an object
        void Update(T entity);

    }
}
