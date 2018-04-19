namespace JobFinder.Web.Areas.Company.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Models;
    using JobFinder.Web.Areas.Company.Models.BussinessCardViewModels;
    using JobFinder.Web.Controllers;
    using JobFinder.Web.Models.MessageViewModels;
    using Microsoft.AspNet.Identity;

    [Authorize(Roles = "Company")]
    public class BusinessCardController : BaseController
    {
        public BusinessCardController(IJobFinderData data) : base(data)
        {
        }

        public ActionResult GetBusinessCard()
        {
            string companyId = this.User.Identity.GetUserId();

            return this.RedirectToAction("GetCompanyBusinessCard", "PublicOffer", new { area = string.Empty, id = companyId });
        }

        [HttpPost]
        public ActionResult EditCard(EditBussinessCardViewModel model)
        {
            string currentCompanyId = User.Identity.GetUserId();

            if (model != null && model.Id != currentCompanyId)
            {
                return this.RedirectToAction("GetBusinessCard", "BusinessCard");
            }

            if (!ModelState.IsValid)
            {
                MessageViewModel message = new MessageViewModel 
                { Text = "Company name is required. Please try again.", Type = MessageType.Error };
                this.TempData["Message"] = message;

                // TempData["InvalidModel"] = "Company name is required. Please try again.";
                return this.RedirectToAction("GetBusinessCard", "BusinessCard");
            }

            if (model.BusinessSectors == null || model.BusinessSectors.Length > 3)
            {
                MessageViewModel message = new MessageViewModel 
                { Text = "Please select at least one business sector and up to 3.", Type = MessageType.Error };
                this.TempData["Message"] = message;

                // TempData["InvalidModel"] = "Please select at least one business sector.";
                return this.RedirectToAction("GetBusinessCard", "BusinessCard");
            }

            JobFinder.Models.Company company = this.Data.Companies.Find(model.Id);
            company.CompanyName = model.CompanyName;
            company.AboutUs = model.AboutUs;
            company.Address = model.Address;
            company.WebSite = model.WebSite;
            this.AddBusinessSectors(model.BusinessSectors, company.Id);

            MessageViewModel successMessage = new MessageViewModel 
            { Text = "Your changes are saved.", Type = MessageType.Success };
            this.TempData["Message"] = successMessage;

            // TempData["Success"] = "Your changes are saved.";
            return this.RedirectToAction("GetBusinessCard", "BusinessCard");
        }

        private void AddBusinessSectors(int[] sectors, string userId)
        {
            var sectorsToAdd = new List<BusinessSector>();

            foreach (var id in sectors)
            {
                var sector = this.Data.BusinessSectors.Find(id);
                sectorsToAdd.Add(sector);
            }

            var user = this.Data.Companies.Find(userId);
            user.BusinessSectors.Clear();
            user.BusinessSectors = sectorsToAdd;
            this.Data.SaveChanges();
        }
    }
}