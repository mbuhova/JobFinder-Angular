namespace JobFinder.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("People")]
    public class Person : User
    {
        public Person()
        {
            this.FollowedOffers = new HashSet<JobOffer>();
            this.Applications = new HashSet<Application>();
        }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public virtual ICollection<JobOffer> FollowedOffers { get; set; }

        public ICollection<Application> Applications { get; set; }
    }
}
