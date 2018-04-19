namespace JobFinder.Web.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Web.Areas.Admin.Models.UserViewModels;
    using JobFinder.Web.Controllers;
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    [Authorize(Roles = "Admin")]
    public class UserController : BaseController
    {
        public UserController(IJobFinderData data) : base(data)
        {
        }

        // GET: Admin/User
        public ActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var model = this.Data.Companies.All().OrderBy(c => c.CompanyName)
                .Select(CompanyViewModel.FromCompany).ToDataSourceResult(request, ModelState);
            return this.Json(model);
        }

        [HttpPost]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, CompanyViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                JobFinder.Models.Company company = this.Data.Companies.Find(model.Id);
                if (company != null)
                {
                   // if (model.IsApproved)
                   // {
                   //     Roles.AddUserToRole(company.UserName, "Company");
                   // }
                   // else
                   // {
                   //     Roles.RemoveUserFromRole(company.UserName, "Company");
                   // }
                    company.IsApproved = model.IsApproved;
                    this.Data.Companies.Update(company);
                }
            }

            return this.Json(new[] { model }.ToDataSourceResult(request, this.ModelState));
        }

        [HttpPost]
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, CompanyViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                JobFinder.Models.Company toDelete = this.Data.Companies.Find(model.Id);
                this.Data.Companies.Delete(toDelete);
            }

            return this.Json(new[] { model }.ToDataSourceResult(request, this.ModelState));
        }
    }
}