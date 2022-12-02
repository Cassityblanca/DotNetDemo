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
        private IRepository<ShoppingCart> _ShoppingCart;
        private IRepository<OrderHeader> _OrderHeader;

        public IRepository<ShoppingCart> ShoppingCart
        {
            get
            {

                _ShoppingCart ??= new Repository<ShoppingCart>(_dbContext);
                return _ShoppingCart;
            }
        }
        public IRepository<OrderHeader> OrderHeader
        {
            get
            {

                _OrderHeader ??= new Repository<OrderHeader>(_dbContext);
                return _OrderHeader;
            }
        }

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

        //New for menuitem assignment
        private IRepository<MenuItem> _MenuItem;
        public IRepository<MenuItem> MenuItem
        {
            get
            {
                _MenuItem ??= new Repository<MenuItem>(_dbContext);
                return _MenuItem;
            }
        }


        private IRepository<ApplicationUser> _ApplicationUser;
        public IRepository<ApplicationUser> ApplicationUser
        {
            get
            {
                _ApplicationUser ??= new Repository<ApplicationUser>(_dbContext);
                return _ApplicationUser;
            }
        }

    }
}

