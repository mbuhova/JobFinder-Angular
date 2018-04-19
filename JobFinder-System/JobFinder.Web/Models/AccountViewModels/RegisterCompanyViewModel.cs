namespace JobFinder.Web.Models.AccountViewModels
{
    using System.ComponentModel.DataAnnotations;

    // Company register
    public class RegisterCompanyViewModel
    {
        [Required]
        [Display(Name = "Bulstat")]
        public string Bulstat { get; set; }

        [Required]
        [Display(Name = "Company name")]
        public string CompanyName { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Url]
        [Display(Name = "Web site")]
        public string WebSite { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 100)]
        [DataType(DataType.Password)]
        [Display(Name = "About us")]
        public string AboutUs { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public int[] SelectedIds { get; set; }
    }
}
