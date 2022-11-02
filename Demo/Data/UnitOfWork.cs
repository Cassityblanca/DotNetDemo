using Demo.Interfaces;
using Demo.Models;

namespace Demo.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        //Dependency Injection of the DB Service

        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        private IRepository<Category> _Category;
        private IRepository<FoodType> _FoodType;

        public IRepository<Category> Category
        {
            get
            {

                _Category ??= new Repository<Category>(_dbContext);
                return _Category;
            }
        }

        public IRepository<FoodType> FoodType
        {
            get
            {

                _FoodType ??= new Repository<FoodType>(_dbContext);
                return _FoodType;
            }
        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public void Commit()
        {
            _dbContext.SaveChanges();
        }
    }
}

