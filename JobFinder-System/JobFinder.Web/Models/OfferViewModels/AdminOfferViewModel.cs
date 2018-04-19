namespace JobFinder.Web.Models.OfferViewModels
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using JobFinder.Models;

    public class AdminOfferViewModel
    {
        public static Expression<Func<JobOffer, AdminOfferViewModel>> FromJobOffer
        {
            get
            {
                return o => new AdminOfferViewModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    DateCreated = o.DateCreated,
                    Description = o.Description,
                    CompanyName = o.Company.CompanyName,
                    IsActive = o.IsActive
                };
            }
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string Title { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string Description { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime DateCreated { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string CompanyName { get; set; }

        public bool IsActive { get; set; }
    }
}
