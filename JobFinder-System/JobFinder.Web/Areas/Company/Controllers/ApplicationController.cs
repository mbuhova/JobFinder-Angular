namespace JobFinder.Web.Areas.Company.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Models;
    using JobFinder.Web.Controllers;
    using JobFinder.Web.Models.ApplicationViewModels;
    using PagedList;

    [Authorize(Roles = "Company")]
    public class ApplicationController : BaseController
    {
        private const int ApplicationsPerPage = 5;

        public ApplicationController(IJobFinderData data) : base(data)
        {
        }

        public ActionResult GetApplications(int? id, int? page, string approved, string rejected, string notSeen)
        {
            int pageNumber = page ?? 1;

            IEnumerable<ApplicationViewModel> model = this.Data.Applications.All()
                .Where(a => a.JobOfferId == (int)id).OrderByDescending(a => a.DateUploaded).Include("JobOffer")
                .Select(ApplicationViewModel.FromApplication);

            model = this.Filter(model, approved, rejected, notSeen);

            model = model.ToPagedList(pageNumber, ApplicationsPerPage);

            this.TempData["approved"] = approved == "on" ? true : false;
            this.TempData["rejected"] = rejected == "on" ? true : false;
            this.TempData["notSeen"] = notSeen == "on" ? true : false;

            return this.View(model);
        }

        public ActionResult ChangeStatus(int id, bool isApproved)
        {
            Application app = this.Data.Applications.Find(id);
            if (app != null)
            {
                app.IsApproved = isApproved;
                this.Data.Applications.Update(app);
            }

            return new EmptyResult();
        }

        public FileContentResult DownloadFile(int id)
        {
            Application cv = this.Data.Applications.All().FirstOrDefault(a => a.Id == id);
            return this.File(cv.FileData, cv.ContentType, cv.FileName);
        }

        private IEnumerable<ApplicationViewModel> Filter(IEnumerable<ApplicationViewModel> model, string appr, string rej, string notseen)
        {
            if (appr == null && rej == null && notseen == null)
            {
                return model;
            }

            bool approved = appr == "on" ? true : false;
            bool rejected = rej == "on" ? true : false;
            bool notSeen = notseen == "on" ? true : false;

            if (approved && rejected && notSeen)
            {
                return model;
            }
            else if (approved && rejected)
            {
                return model.Where(m => m.IsApproved != null);
            }
            else if (approved && notSeen)
            {
                return model.Where(m => m.IsApproved != false);
            }
            else if (rejected && notSeen)
            {
                return model.Where(m => m.IsApproved != true);
            }
            else if (approved)
            {
                return model.Where(m => m.IsApproved == true);
            }
            else if (rejected)
            {
                return model.Where(m => m.IsApproved == false);
            }
            else if (notSeen)
            {
                return model.Where(m => m.IsApproved == null);
            }

            return model;
        }
    }
}