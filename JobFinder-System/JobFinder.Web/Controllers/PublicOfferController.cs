namespace JobFinder.Web.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Models;
    using JobFinder.Web.Areas.Company.Models.BussinessCardViewModels;
    using JobFinder.Web.Models.MessageViewModels;
    using JobFinder.Web.Models.OfferViewModels;
    using Microsoft.AspNet.Identity;

    public class PublicOfferController : BaseController
    {
        public PublicOfferController(IJobFinderData data) : base(data)
        {
        }

        // GET: PublicOffer
        public ActionResult OfferDetails(int? id)
        {
            JobOffer offer = this.Data.JobOffers.Find((int)id);

            if (id == null || offer == null)
            {
                return this.RedirectToAction("SearchOffers", "SearchOffer");
            }

            offer.Views += 1;
            this.Data.JobOffers.Update(offer);
            
            DetailsOfferViewModel model = this.Data.JobOffers.All().Where(o => o.Id == id)
                .Select(DetailsOfferViewModel.FromJobOffer).FirstOrDefault();

            if (Request.IsAuthenticated && User.IsInRole("Person"))
            {
                string personId = this.User.Identity.GetUserId();
                Application app = this.Data.Applications.All().Where(a => a.PersonId == personId && a.JobOfferId == model.Id)
                    .Include("Person").FirstOrDefault();
                this.TempData["HasApplied"] = (app == null) ? false : true;
                Person current = this.Data.People.Find(personId);

                if (offer.PeopleFollowing.Contains(current))
                {
                    this.TempData["FollowButtonText"] = "Unfollow";
                }
                else
                {
                    this.TempData["FollowButtonText"] = "Follow";
                }
            }

            return this.View(model);
        }

        public ActionResult GetCompanyBusinessCard(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return this.RedirectToAction("SearchOffers", "SearchOffer");
            }

            BussinessCardViewModel model = this.Data.Companies.All().Where(c => c.Id == id)
                .Select(BussinessCardViewModel.FromCompany).FirstOrDefault();

            this.TempData["Sectors"] = this.Data.BusinessSectors.All()
                .Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            if (model == null)
            {
                MessageViewModel message = new MessageViewModel { Type = MessageType.Error, Text = "Company not found." };
                this.TempData["Message"] = message;
                return this.RedirectToAction("SearchOffers", "SearchOffer");

                // TempData["NotFound"] = "Company not found.";
            }

            this.TempData["showEditBtn"] = false;
            string userId = this.User.Identity.GetUserId();

            if (userId == id)
            {
                this.TempData["showEditBtn"] = true;
            }

            return this.View("BussinessCard", model);
        }
    }
}