namespace JobFinder.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Companies")]
    public class Company : User
    {
        public Company()
        {
            this.BusinessSectors = new HashSet<BusinessSector>();
        }

        [Required]
        [Index(IsUnique = true)]
        [MaxLength(13)]
        public string Bulstat { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [MaxLength(60)]
        public string CompanyName { get; set; }

        public virtual ICollection<BusinessSector> BusinessSectors { get; set; }

        public string Address { get; set; }

        public string WebSite { get; set; }

        public string AboutUs { get; set; }

        public bool IsApproved { get; set; }
    }
}
