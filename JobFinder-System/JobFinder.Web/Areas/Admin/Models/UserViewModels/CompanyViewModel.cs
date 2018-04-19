namespace JobFinder.Web.Areas.Admin.Models.UserViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq.Expressions;
    using System.Web.Mvc;

    public class CompanyViewModel
    {
        public static Expression<Func<JobFinder.Models.Company, CompanyViewModel>> FromCompany
        {
            get
            {
                return c => new CompanyViewModel
                {
                    Id = c.Id,
                    Name = c.CompanyName,
                    Bulstat = c.Bulstat,
                    Email = c.Email,
                    IsApproved = c.IsApproved
                };
            }
        }

        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public string Bulstat { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public string Name { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public string Email { get; set; }

        public bool IsApproved { get; set; }
    }
}
