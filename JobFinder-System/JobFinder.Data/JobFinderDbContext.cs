namespace JobFinder.Data
{
    using System.Data.Entity;
    using JobFinder.Data.Migrations;
    using JobFinder.Models;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class JobFinderDbContext : IdentityDbContext<User>
    {
        public JobFinderDbContext()
            : base("JobFinderConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<JobFinderDbContext, Configuration>());
        }

        public IDbSet<BusinessSector> BusinessSectors { get; set; }

        public IDbSet<Person> People { get; set; }

        public IDbSet<Company> Companies { get; set; }

        public IDbSet<Application> Applications { get; set; }

        public IDbSet<JobOffer> JobOffers { get; set; }

        public IDbSet<Town> Towns { get; set; }

        public static JobFinderDbContext Create()
        {
            return new JobFinderDbContext();
        }
    }
}
