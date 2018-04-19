namespace JobFinder.Web.Areas.Person.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Models;
    using JobFinder.Web.Controllers;
    using JobFinder.Web.Models.ApplicationViewModels;
    using JobFinder.Web.Models.MessageViewModels;
    using Microsoft.AspNet.Identity;
    using PagedList;

    [Authorize(Roles = "Person")]
    public class ApplyController : BaseController
    {
        private const int MaxFileSize = 1024 * 1024;

        private const int PageSize = 5;

        public ApplyController(IJobFinderData data) : base(data)
        {
        }

        public ActionResult ApplyForOffer()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyForOffer(HttpPostedFileBase file)
        {
            bool isValid = this.IsValidRequest(file);

            if (!this.IsValidRequest(file))
            {
                return this.RedirectToAction("ApplyForOffer");
            }

            int offerId = int.Parse(RouteData.Values["id"].ToString());
            Application cv = new Application();

            byte[] fileData = new byte[file.InputStream.Length];
            file.InputStream.Read(fileData, 0, Convert.ToInt32(file.InputStream.Length));
            cv.FileData = fileData;
            cv.FileName = file.FileName;
            cv.ContentType = file.ContentType;
            cv.FileSize = file.ContentLength;
            cv.DateUploaded = DateTime.Now;
            cv.PersonId = User.Identity.GetUserId();
            cv.JobOfferId = offerId;
            this.Data.Applications.Add(cv);

            var offer = this.Data.JobOffers.Find(offerId);
            offer.ApplicationsCount += 1;
            this.Data.JobOffers.Update(offer);

            MessageViewModel message = new MessageViewModel 
            { Text = "Your application was successfull. Good luck!", Type = MessageType.Success };
            this.TempData["Message"] = message;
            return this.RedirectToAction("ApplyForOffer");
        }

        public ActionResult MyApplications(int? page)
        {
            string personId = User.Identity.GetUserId();

            IEnumerable<ApplicationViewModel> model = this.Data.Applications.All().Where(d => d.PersonId == personId)
                .AsQueryable().Include("Company").Select(ApplicationViewModel.FromApplication)
                .OrderByDescending(a => a.DateUploaded);
            int pageNumber = page ?? 1;
            model = model.ToPagedList(pageNumber, PageSize);
            return this.View(model);
        }

        public FileContentResult DownloadFile(int id)
        {
            string personId = User.Identity.GetUserId();

            Application cv = this.Data.Applications.All().FirstOrDefault(a => a.Id == id && a.PersonId == personId);

            // string mimeType = "application/pdf";
            // Response.AppendHeader("Content-Disposition", "inline; filename=" + cv.FileName);
            // return File(cv.FileData, mimeType);
            return this.File(cv.FileData, cv.ContentType, cv.FileName);
        }

        private bool IsValidRequest(HttpPostedFileBase file)
        {
            string personId = User.Identity.GetUserId();
            var routeId = RouteData.Values["id"];
            MessageViewModel message = null;

            if (routeId == null)
            {
                message = new MessageViewModel { Type = MessageType.Error, Text = "Select a offer" };
                this.TempData["Message"] = message;
                return false;
            }

            string id = routeId.ToString();

            if (string.IsNullOrEmpty(id))
            {
                message = new MessageViewModel { Type = MessageType.Error, Text = "Select a offer" };
                this.TempData["Message"] = message;
                return false;
            }

            int offerId = int.Parse(id);
            Application doc = this.Data.Applications.All().Where(d => d.JobOfferId == offerId && d.PersonId == personId).FirstOrDefault();

            if (doc != null)
            {
                message = new MessageViewModel { Type = MessageType.Error, Text = "You have already applied for this offer." };
                this.TempData["Message"] = message;
                return false;
            }

            JobOffer offer = this.Data.JobOffers.Find(offerId);
            if (offer == null)
            {
                message = new MessageViewModel { Type = MessageType.Error, Text = "Job offer you are appling for doesnt exist." };
                this.TempData["Message"] = message;
                return false;
            }

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };

            if (file == null)
            {
                message = new MessageViewModel { Type = MessageType.Error, Text = "Please select a file to upload (.doc, .docx or .pdf format)." };
                this.TempData["Message"] = message;
                return false;
            }

            var extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension))
            {
                message = new MessageViewModel { Type = MessageType.Error, Text = "Please upload file in .doc, .docx or .pdf format." };
                this.TempData["Message"] = message;
                return false;
            }

            if (file.ContentLength > MaxFileSize)
            {
                message = new MessageViewModel { Type = MessageType.Error, Text = "Please upload file with size less than 1 MB." };
                this.TempData["Message"] = message;
                return false;
            }

            return true;
        }
    }
}