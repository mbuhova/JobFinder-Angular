namespace JobFinder.Web.Areas.Company.Models.JobOfferViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class CreateOfferViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int TownId { get; set; }

        [Required]
        public int BusinessSectorId { get; set; }

        [Required]
        [Display(Name = "Permanent")]
        public bool IsPermanent { get; set; }

        [Required]
        [Display(Name = "Temporary")]
        public bool IsTemporary { get; set; }

        [Required]
        [Display(Name = "Full Time")]
        public bool IsFullTime { get; set; }

        [Required]
        [Display(Name = "Part Time")]
        public bool IsPartTime { get; set; }
    }
}
