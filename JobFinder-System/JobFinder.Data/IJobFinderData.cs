namespace JobFinder.Data
{
    using JobFinder.Data.Repositories;
    using JobFinder.Models;

    public interface IJobFinderData
    {
        IRepository<User> Users { get; }

        IRepository<Person> People { get; }

        IRepository<Company> Companies { get; }

        IRepository<BusinessSector> BusinessSectors { get; }

        IRepository<Application> Applications { get; }

        IRepository<JobOffer> JobOffers { get; }

        IRepository<Town> Towns { get; }

        void SaveChanges();
    }
}
