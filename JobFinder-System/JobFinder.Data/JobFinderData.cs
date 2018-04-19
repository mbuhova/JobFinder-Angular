namespace JobFinder.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using JobFinder.Data.Repositories;
    using JobFinder.Models;

    public class JobFinderData : IJobFinderData
    {
        private DbContext context;

        private IDictionary<Type, object> repositories;

        public JobFinderData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<User> Users
        {
            get { return this.GetRepository<User>(); }
        }

        public IRepository<Person> People
        {
            get { return this.GetRepository<Person>(); }
        }

        public IRepository<Company> Companies
        {
            get { return this.GetRepository<Company>(); }
        }

        public IRepository<BusinessSector> BusinessSectors
        {
            get { return this.GetRepository<BusinessSector>(); }
        }

        public IRepository<Application> Applications
        {
            get { return this.GetRepository<Application>(); }
        }

        public IRepository<JobOffer> JobOffers
        {
            get { return this.GetRepository<JobOffer>(); }
        }

        public IRepository<Town> Towns
        {
            get { return this.GetRepository<Town>(); }
        }

        public void SaveChanges()
        {
            this.context.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            var typeOfRepository = typeof(T);
            if (!this.repositories.ContainsKey(typeOfRepository))
            {
                var newRepository = Activator.CreateInstance(typeof(EFRepository<T>), this.context);
                this.repositories.Add(typeOfRepository, newRepository);
            }

            return (IRepository<T>)this.repositories[typeOfRepository];
        }
    }
}
