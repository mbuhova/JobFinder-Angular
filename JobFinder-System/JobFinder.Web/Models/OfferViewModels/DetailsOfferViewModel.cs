namespace JobFinder.Web.Models.OfferViewModels
{
    using System;
    using System.Linq.Expressions;
    using JobFinder.Models;

    public class DetailsOfferViewModel
    {
        public static Expression<Func<JobOffer, DetailsOfferViewModel>> FromJobOffer
        {
            get
            {
                return o => new DetailsOfferViewModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    Description = o.Description,
                    DateCreated = o.DateCreated,
                    Views = o.Views,
                    Town = o.Town.Name,
                    ApplicationsCount = o.ApplicationsCount,
                    BusinessSector = o.BusinessSector.Name,
                    IsActive = o.IsActive,
                    CompanyName = o.Company.CompanyName,
                    IsFullTime = o.IsFullTime,
                    IsPermanent = o.IsPermanent
                };
            }
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime DateCreated { get; set; }

        public string Description { get; set; }

        public int Views { get; set; }

        public int ApplicationsCount { get; set; }

        public string Town { get; set; }

        public string BusinessSector { get; set; }

        public bool IsActive { get; set; }

        public string CompanyName { get; set; }

        public bool? IsFullTime { get; set; }

        public bool? IsPermanent { get; set; }
    }
}
