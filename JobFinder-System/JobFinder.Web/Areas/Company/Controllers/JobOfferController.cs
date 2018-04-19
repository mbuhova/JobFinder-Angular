namespace JobFinder.Web.Areas.Company.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Models;
    using JobFinder.Web.Areas.Company.Models.JobOfferViewModels;
    using JobFinder.Web.Controllers;
    using JobFinder.Web.Helpers;
    using JobFinder.Web.Models.MessageViewModels;
    using JobFinder.Web.Models.OfferViewModels;
    using Microsoft.AspNet.Identity;
    using PagedList;

    [Authorize(Roles = "Company")]
    public class JobOfferController : BaseController
    {
        private const int OffersPerPage = 10;

        public JobOfferController(IJobFinderData data) : base(data)
        {            
        }

        public ActionResult GetOffers(int? page, bool? onlyActive)
        {
            string companyId = User.Identity.GetUserId();
            int pageNumber = page ?? 1;

            IEnumerable<ListOfferViewModel> model = this.Data.JobOffers.All().Where(o => o.CompanyId == companyId)
                .OrderByDescending(o => o.DateCreated).Select(ListOfferViewModel.FromJobOffer);

            if (onlyActive != null)
            {
                model = model.Where(m => m.IsActive == onlyActive);
                this.TempData["onlyActive"] = onlyActive;
            }

            model = model.ToPagedList(pageNumber, OffersPerPage);
            return this.View(model);
        }

        public ActionResult OfferDetails(int? id)
        {
            if (id == null)
            {
                return this.RedirectToAction("GetOffers");
            }

            string companyId = User.Identity.GetUserId();            
            DetailsOfferViewModel model = this.Data.JobOffers.All().Where(o => o.Id == id && o.CompanyId == companyId)
                .Select(DetailsOfferViewModel.FromJobOffer).FirstOrDefault();

            if (model == null)
            {
                MessageViewModel message = new MessageViewModel { Text = "Job offer not found.", Type = MessageType.Error };
                this.TempData["Message"] = message;
                return this.RedirectToAction("GetOffers", "JobOffer");

                // TempData["NotFound"] = "Job offer not found.";
            }

            return this.View(model);
        }

        public ActionResult CreateOffer()
        {
            this.TempData["Towns"] = this.Data.Towns.All().Where(t => !t.IsDeleted).Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() });
            this.TempData["BusinessSectors"] = this.Data.BusinessSectors.All().Where(s => !s.IsDeleted).Select(b => new SelectListItem { Text = b.Name, Value = b.Id.ToString() });
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOffer(CreateOfferViewModel model)
        {
            if (ModelState.IsValid)
            {
                string companyId = User.Identity.GetUserId();

                JobOffer offer = new JobOffer();
                offer.Title = model.Title;
                offer.Description = model.Description;
                offer.DateCreated = DateTime.Now;
                offer.TownId = model.TownId;
                offer.IsActive = true;
                offer.CompanyId = companyId;
                offer.BusinessSectorId = model.BusinessSectorId;
                offer.IsPermanent = model.IsPermanent.MapBools(model.IsTemporary);
                offer.IsFullTime = model.IsFullTime.MapBools(model.IsPartTime);
                this.Data.JobOffers.Add(offer);
                MessageViewModel message = new MessageViewModel 
                    { Type = MessageType.Success, Text = "You have successfully created your job offer." };
                this.TempData["Message"] = message;

                // TempData["Success"] = "You have successfully created your job offer.";
                return this.RedirectToAction("CreateOffer");
            }

            return this.View(model);
        }

        public ActionResult MarkAsExpired(int? id)
        {
            if (id != null)
            {
                JobOffer offer = this.Data.JobOffers.Find((int)id);
                if (offer != null)
                {
                    offer.IsActive = false;
                    this.Data.JobOffers.Update(offer);
                }               
            }

            return new EmptyResult();
        }
    }
}