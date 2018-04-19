namespace JobFinder.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class BusinessSector
    {
        public BusinessSector()
        {
            this.Companies = new HashSet<Company>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [MaxLength(60)]
        public string Name { get; set; }

        public virtual ICollection<Company> Companies { get; set; }

        public bool IsDeleted { get; set; }
    }
}
