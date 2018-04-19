namespace JobFinder.Web.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Models;
    using JobFinder.Web.Controllers;
    using JobFinder.Web.Models.OfferViewModels;
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    [Authorize(Roles = "Admin")]
    public class OfferController : BaseController
    {
        public OfferController(IJobFinderData data) : base(data)
        {
        }

        // GET: Admin/Offer
        public ActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var model = this.Data.JobOffers.All().OrderByDescending(o => o.DateCreated)
                .Select(AdminOfferViewModel.FromJobOffer).ToDataSourceResult(request, ModelState);
            return this.Json(model);
        }

        [HttpPost]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, AdminOfferViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                JobOffer offer = this.Data.JobOffers.Find(model.Id);
                if (offer != null)
                {
                    // update only status
                    offer.IsActive = model.IsActive;
                    this.Data.JobOffers.Update(offer);
                }
            }

            return this.Json(new[] { model }.ToDataSourceResult(request, this.ModelState));
        }
    }
}