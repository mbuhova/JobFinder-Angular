namespace JobFinder.Web.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Models;
    using JobFinder.Web.Areas.Admin.Models.TownViewModels;
    using JobFinder.Web.Controllers;
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    [Authorize(Roles = "Admin")]
    public class TownController : BaseController
    {
        public TownController(IJobFinderData data) : base(data)
        {
        }

        // GET: Admin/Town
        public ActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var model = this.Data.Towns.All().Where(t => !t.IsDeleted).OrderBy(t => t.Name)
                .Select(t => new TownViewModel { Id = t.Id, Name = t.Name }).ToDataSourceResult(request, ModelState);
            return this.Json(model);
        }

        [HttpPost]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, TownViewModel model)
        {
           if (model != null && ModelState.IsValid)
           {
               Town town = this.Data.Towns.All().Where(t => t.Name == model.Name).FirstOrDefault();
               if (town == null)
               {
                   Town toAdd = new Town { Name = model.Name };
                   this.Data.Towns.Add(toAdd);
                   model.Id = toAdd.Id;
               } 
           }

           return this.Json(new[] { model }.ToDataSourceResult(request, this.ModelState));
        }

        [HttpPost]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, TownViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                Town town = this.Data.Towns.All().Where(t => t.Name == model.Name).FirstOrDefault();
                if (town == null)
                {
                    Town toUpdate = this.Data.Towns.Find(model.Id);
                    toUpdate.Name = model.Name;
                    this.Data.Towns.Update(toUpdate);
                }
            }

            return this.Json(new[] { model }.ToDataSourceResult(request, this.ModelState));
        }

        [HttpPost]
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, TownViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                Town toDelete = this.Data.Towns.Find(model.Id);
                toDelete.IsDeleted = true;
                this.Data.Towns.Update(toDelete);

                // this.data.Towns.Delete(model.Id);
            }

            return this.Json(new[] { model }.ToDataSourceResult(request, this.ModelState));
        }
    }
}