using Demo.Models;

namespace Demo.Interfaces
{
    public interface IUnitOfWork
    {
        //Data Accessors
        public IRepository<Category> Category { get; }
        public IRepository<FoodType> FoodType { get; }
        public IRepository<MenuItem> MenuItem { get; }
        public IRepository<ApplicationUser> ApplicationUser { get; }


        //save changes to data source
        void Commit();
        //same but an Asynchronous Commit
        Task<int> CommitAsync();

    }
}
