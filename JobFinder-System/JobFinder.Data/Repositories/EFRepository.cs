namespace JobFinder.Data.Repositories
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class EFRepository<T> : IRepository<T> where T : class
    {
        public EFRepository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("An instance of DbContext is required to use this repository.", "context");
            }

            this.Context = context;
            this.Set = this.Context.Set<T>();
        }

        protected IDbSet<T> Set { get; set; }

        protected DbContext Context { get; set; }

        public IQueryable<T> All()
        {
            return this.Set;
        }

        public T Find(int id)
        {
            return this.Set.Find(id);
        }

        public T Find(string id)
        {
            return this.Set.Find(id);
        }

        public void Add(T item)
        {
            this.ChangeState(item, EntityState.Added);
            this.SaveChanges();
        }

        public void Update(T item)
        {
            this.ChangeState(item, EntityState.Modified);
            this.SaveChanges();
        }

        public T Delete(int id)
        {
            var entity = this.Find(id);
            this.Delete(entity);
            return entity;
        }

        public T Delete(T item)
        {
            this.ChangeState(item, EntityState.Deleted);
            this.SaveChanges();
            return item;
        }

        public void SaveChanges()
        {
            this.Context.SaveChanges();
        }

        private void ChangeState(T entity, EntityState state)
        {
            var entry = this.Context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this.Set.Attach(entity);
            }

            entry.State = state;
        }
    }
}
