using Microsoft.EntityFrameworkCore;
using Demo.Data;
using Demo.Interfaces;
using System.Linq.Expressions;

namespace Demo.Models
{
    public class Repository<T> : IRepository<T> where T : class

    {
        private readonly ApplicationDbContext db_context;  //Contection to DB
        internal DbSet<T> dbset; //used to hold a set of date for GetAll()
        public Repository(ApplicationDbContext ctx)
        {
            db_context = ctx;
          
            dbset = db_context.Set<T>();

        }

        
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public void Add(T entity)
        {
            db_context.Set<T>().Add(entity);
            db_context.SaveChanges();
        }

        public void Delete(T entity)
        {
            db_context.Set<T>().Remove(entity);
            db_context.SaveChanges();
        }

        public void Delete(IEnumerable<T> entities)
        {
            db_context.Set<T>().RemoveRange(entities);
            db_context.SaveChanges();
        }

        public virtual T Get(Expression<Func<T, bool>> predicate, bool asNotTracking = false, string includes = null)
        {
            if (includes == null)
            {
                if (asNotTracking)
                {
                    return db_context.Set<T>()
                                     .AsNoTracking()
                                     .Where(predicate)
                                     .FirstOrDefault();
                }
                else
                {
                    return db_context.Set<T>()
                        .Where(predicate)
                        .FirstOrDefault();
                }
            }
            else
            {
                IQueryable<T> queryable = db_context.Set<T>();
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }

                if (asNotTracking)
                {
                    return queryable
                        .AsNoTracking()
                        .Where(predicate)
                        .FirstOrDefault();
                }
                else
                {
                    return queryable
                        .Where(predicate)
                        .FirstOrDefault();
                }
            }
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool asNotTracking = false, string includes = null)
        {
            if (includes == null)
            {
                if (asNotTracking)
                {
                    return await db_context.Set<T>()
                        .AsNoTracking()
                        .Where(predicate)
                        .FirstOrDefaultAsync();
                }
                else
                {
                    return await db_context.Set<T>()
                        .Where(predicate)
                        .FirstOrDefaultAsync();
                }
            }
            else
            {
                IQueryable<T> queryable = db_context.Set<T>();
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }

                if (asNotTracking)
                {
                    return await queryable
                        .AsNoTracking()
                        .Where(predicate)
                        .FirstOrDefaultAsync();
                }
                else
                {
                    return await queryable
                        .Where(predicate)
                        .FirstOrDefaultAsync();
                }
            }
        }

        // The virtual keyword is used to modify a method, property, indexer, or
        // and allows for it to be overriden in a derived class.
        public virtual T GetById(int id)
        {
            return db_context.Set<T>().Find(id);
        }

        public virtual IEnumerable<T> List()
        {
            return db_context.Set<T>().ToList().AsEnumerable();
        }

        public virtual IEnumerable<T> List(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> orderBy = null, string includes = null)
        {
            IQueryable<T> queryable = db_context.Set<T>();
            if (predicate != null && includes == null)
            {
                return db_context.Set<T>()
                    .Where(predicate)
                    .AsEnumerable();
            }
            // have includes
            else if (includes != null)
            {
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }
            }

            if (predicate == null)
            {
                if (orderBy == null)
                {
                    return queryable.AsEnumerable();
                }
                else
                {
                    return queryable.OrderBy(orderBy).ToList().AsEnumerable();
                }
            }
            else
            {
                if (orderBy == null)
                {

                    return queryable.OrderBy(orderBy).ToList().AsEnumerable();

                }
                else
                {
                    return queryable.Where(predicate).OrderBy(orderBy).ToList().AsEnumerable();
                }
            }
        }

        public virtual async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> orderBy = null, string includes = null)
        {
            IQueryable<T> queryable = db_context.Set<T>();
            if (predicate != null && includes == null)
            {
                return db_context.Set<T>()
                    .Where(predicate)
                    .AsEnumerable();
            }
            // have includes
            else if (includes != null)
            {
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }
            }

            if (predicate == null)
            {
                if (orderBy == null)
                {
                    return queryable.AsEnumerable();
                }
                else
                {
                    return await queryable.OrderBy(orderBy).ToListAsync();
                }
            }
            else
            {
                if (orderBy == null)
                {

                    return await queryable.OrderBy(orderBy).ToListAsync();

                }
                else
                {
                    return await queryable.Where(predicate).OrderBy(orderBy).ToListAsync();
                }
            }
        }

        public void Update(T entity)
        {
            db_context.Entry(entity).State = EntityState.Modified;
            db_context.SaveChanges();
        }


    }
}