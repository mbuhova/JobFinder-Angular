namespace JobFinder.Web.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Models;
    using JobFinder.Web.Areas.Admin.Models.BusinessSectorViewModels;
    using JobFinder.Web.Controllers;
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    [Authorize(Roles = "Admin")]
    public class BusinessSectorController : BaseController
    {
        public BusinessSectorController(IJobFinderData data) : base(data)
        {
        }

        // GET: Admin/BusinessSector
        public ActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var model = this.Data.BusinessSectors.All().Where(s => !s.IsDeleted).OrderBy(s => s.Name)
                .Select(s => new BusinessSectorViewModel { Id = s.Id, Name = s.Name }).ToDataSourceResult(request, ModelState);
            return this.Json(model);
        }

        [HttpPost]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, BusinessSectorViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                BusinessSector town = this.Data.BusinessSectors.All().Where(s => s.Name == model.Name).FirstOrDefault();
                if (town == null)
                {
                    BusinessSector toAdd = new BusinessSector { Name = model.Name };
                    this.Data.BusinessSectors.Add(toAdd);
                    model.Id = toAdd.Id;
                }
            }

            return this.Json(new[] { model }.ToDataSourceResult(request, this.ModelState));
        }

        [HttpPost]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, BusinessSectorViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                BusinessSector sector = this.Data.BusinessSectors.All().Where(s => s.Name == model.Name).FirstOrDefault();
                if (sector == null)
                {
                    BusinessSector toUpdate = this.Data.BusinessSectors.Find(model.Id);
                    toUpdate.Name = model.Name;
                    this.Data.BusinessSectors.Update(toUpdate);
                }
            }

            return this.Json(new[] { model }.ToDataSourceResult(request, this.ModelState));
        }

        [HttpPost]
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, BusinessSectorViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                BusinessSector toDelete = this.Data.BusinessSectors.Find(model.Id);
                toDelete.IsDeleted = true;
                this.Data.BusinessSectors.Update(toDelete);

                // this.data.Towns.Delete(model.Id);
            }

            return this.Json(new[] { model }.ToDataSourceResult(request, this.ModelState));
        }
    }
}