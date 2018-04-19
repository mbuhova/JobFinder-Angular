namespace JobFinder.Web.Areas.Person.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Models;
    using JobFinder.Web.Controllers;
    using JobFinder.Web.Models.OfferViewModels;
    using Microsoft.AspNet.Identity;
    using PagedList;

    [Authorize(Roles = "Person")]

    public class FollowedOffersController : BaseController
    {
        private const int OffersPerPage = 5;

        public FollowedOffersController(IJobFinderData data) : base(data)
        {
        }

        public ActionResult Follow(int? id)
        {
            JobOffer offer = this.Data.JobOffers.Find((int)id);
            string personId = this.User.Identity.GetUserId();
            JobFinder.Models.Person currentUser = this.Data.People.Find(personId);

            if (offer.PeopleFollowing.Contains(currentUser))
            {
                offer.PeopleFollowing.Remove(currentUser);
            }
            else
            {
                offer.PeopleFollowing.Add(currentUser);
            }

            this.Data.SaveChanges();

            return new EmptyResult();
        }

        public ActionResult GetFollowedOffers(int? page)
        {
            string currentUser = this.User.Identity.GetUserId();
            IEnumerable<SearchResultOfferViewModel> model = this.Data.People.Find(currentUser)
                .FollowedOffers.AsQueryable().Select(SearchResultOfferViewModel.FromJobOffer)
                .OrderByDescending(o => o.DateCreated);

            int pageNumber = page ?? 1;
            model = model.ToPagedList(pageNumber, OffersPerPage);
            return this.View(model);
        }
    }
}