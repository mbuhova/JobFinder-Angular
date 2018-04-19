namespace JobFinder.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Application
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] FileData { get; set; }

        public long FileSize { get; set; }

        [Required]
        public DateTime DateUploaded { get; set; }

        public bool? IsApproved { get; set; }

        [Required]
        public string PersonId { get; set; }

        public virtual Person Person { get; set; }

        [Required]
        public int JobOfferId { get; set; }

        public virtual JobOffer JobOffer { get; set; }
    }
}
