namespace JobFinder.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Web.Models.DashboardModels;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public class DashboardController : BaseController
    {
        public DashboardController(IJobFinderData data)
            : base(data)
        {
        }

        // GET: Dashboard
        public ActionResult Index()
        {
            return this.View(this.GetOffersBySector());
        }

        [HttpPost]
        public ActionResult OffersBySector()
        {
            return this.Json(this.ToJson(this.GetOffersBySector()));
        }

        [HttpPost]
        public ActionResult OffersByTown()
        {
            var townModels = this.Data.JobOffers.All().GroupBy(o => o.TownId)
                .Select(o => new TownViewModel
                {
                    Name = o.FirstOrDefault().Town.Name,
                    Offers = o.GroupBy(off => off.DateCreated.Year).OrderBy(off => off.Key).Select(off =>
                        new OfferViewModel
                        {
                            Year = off.Key,
                            OffersCount = off.Count()
                        })
                }).OrderBy(t => t.Name).ToList();

            var model = new DashboardColumnViewModel();

            foreach (var town in townModels)
            {
                model.Categories.Add(town.Name);

                foreach (var offer in town.Offers)
                {
                    var yearName = string.Format("Year: {0}", offer.Year);
                    var yearSeries = model.Series.FirstOrDefault(s => s.Name == yearName);

                    if (yearSeries == null)
                    {
                        yearSeries = new ColumnViewModel { Name = yearName };
                        model.Series.Add(yearSeries);
                    }

                    yearSeries.Data.Add(offer.OffersCount);
                }
            }

            return this.Json(this.ToJson(model));
        }

        [HttpPost]
        public ActionResult OffersByType()
        {
            var permanentOffers = this.Data.JobOffers.All()
                .Where(o => o.IsActive && (o.IsPermanent == null || o.IsPermanent == true)).Count();
            var temporaryOffers = this.Data.JobOffers.All()
                .Where(o => o.IsActive && (o.IsPermanent == null || o.IsPermanent == false)).Count();
            var fullTimeOffers = this.Data.JobOffers.All()
                .Where(o => o.IsActive && (o.IsFullTime == null || o.IsFullTime == true)).Count();
            var partTimeOffers = this.Data.JobOffers.All()
                .Where(o => o.IsActive && (o.IsFullTime == null || o.IsFullTime == false)).Count();

            var model = new List<ColumnViewModel>
            {
                new ColumnViewModel { Name = "Permanent", Data = new[] { permanentOffers } },
                new ColumnViewModel { Name = "Temporary", Data = new[] { temporaryOffers } },
                new ColumnViewModel { Name = "Full Time", Data = new[] { fullTimeOffers } },
                new ColumnViewModel { Name = "Part Time", Data = new[] { partTimeOffers } }
            };

            return this.Json(this.ToJson(model));
        }

        [HttpPost]
        public ActionResult OffersByTopSector()
        {
            var offers = this.Data.JobOffers.All()
                .Where(o => o.DateCreated >= new DateTime(2017, 1, 1) && o.DateCreated < new DateTime(2018, 1, 1))
                .GroupBy(o => o.BusinessSectorId)
                .OrderByDescending(o => o.Count())
                .Take(5)
                .ToList();

            var model = new List<ColumnViewModel>();

            foreach (var offer in offers)
            {
                var column = new ColumnViewModel { Name = offer.First().BusinessSector.Name };
                var offersByMonth = offer.ToList();

                for (int i = 1; i <= 12; i++)
                {
                    var offersForMonth = offersByMonth.Where(o => o.DateCreated.Month == i).Count();

                    column.Data.Add(offersForMonth);
                }

                model.Add(column);
            }

            return this.Json(this.ToJson(model));
        }

        [HttpPost]
        public ActionResult OffersByTopCompany()
        {
            var offerByTopCompanies = this.Data.JobOffers.All()
                .Where(o => o.DateCreated >= new DateTime(2017, 1, 1) && o.DateCreated < new DateTime(2018, 1, 1))
                .GroupBy(o => o.CompanyId)
                .OrderByDescending(o => o.Count())
                .Take(10)
                .Select(o => new DashboardViewModel { Name = o.FirstOrDefault().Company.CompanyName, Y = o.Count() })
                .ToList();

            var model = new List<object>();

            foreach (var offer in offerByTopCompanies)
            {
                model.Add(new List<object> { offer.Name, offer.Y });
            }

            return this.Json(this.ToJson(model));
        }

        private IEnumerable<DashboardViewModel> GetOffersBySector()
        {
            var jobOffersCount = this.Data.JobOffers.All().Count();

            if (jobOffersCount == 0)
            {
                return new DashboardViewModel[] { };
            }

            var model = this.Data.JobOffers.All().Where(o => o.IsActive).GroupBy(o => o.BusinessSectorId)
                .Select(o => new DashboardViewModel { Name = o.FirstOrDefault().BusinessSector.Name, Y = o.Count() })
                .OrderBy(o => o.Y)
                .Take(15)
                .ToList()
                .Select(o => new DashboardViewModel { Name = o.Name, Y = Math.Round(Convert.ToDouble(o.Y / jobOffersCount) * 100, 2) })
                .ToList();

            return model;
        }

        private object ToJson(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            settings.Converters.Add(new JavaScriptDateTimeConverter());
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}
