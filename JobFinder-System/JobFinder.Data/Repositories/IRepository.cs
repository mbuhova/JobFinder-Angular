namespace JobFinder.Data.Repositories
{
    using System.Linq;

    public interface IRepository<T> where T : class
    {
        IQueryable<T> All();

        T Find(int id);

        T Find(string id);

        void Add(T item);

        void Update(T item); 

        T Delete(int id); 

        T Delete(T item);

        void SaveChanges();
    }
}
