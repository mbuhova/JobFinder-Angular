namespace JobFinder.Web.Areas.Company.Models.JobOfferViewModels
{
    using System;
    using System.Linq.Expressions;
    using JobFinder.Models;

    public class ListOfferViewModel
    {
        public static Expression<Func<JobOffer, ListOfferViewModel>> FromJobOffer
        {
            get
            {
                return o => new ListOfferViewModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    DateCreated = o.DateCreated,
                    IsActive = o.IsActive
                };
            }
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }
    }
}
