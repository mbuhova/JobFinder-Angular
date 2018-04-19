namespace JobFinder.Web.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Models;
    using JobFinder.Web.Helpers;
    using JobFinder.Web.Models.OfferViewModels;
    using PagedList;

    public class SearchOfferController : BaseController
    {
        private const int OffersPerPage = 5;

        public SearchOfferController(IJobFinderData data) : base(data)
        {
        }

        public ActionResult SearchOffers()
        {
            IEnumerable<SelectListItem> towns = this.Data.Towns.All().Where(t => !t.IsDeleted)
                .Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() })
                .OrderBy(t => t.Text);
            IEnumerable<SelectListItem> businessSectors = this.Data.BusinessSectors.All().Where(s => !s.IsDeleted)
                .Select(b => new SelectListItem { Text = b.Name, Value = b.Id.ToString() })
                .OrderBy(b => b.Text);
            this.TempData["Towns"] = towns;
            this.TempData["BusinessSectors"] = businessSectors;
            return this.View();
        }
        
        public ActionResult OfferSearchResults(int? page, int[] sectors, int? town, string word, bool isPermanent, bool isTemporary, bool isFullTime, bool isPartTime)
        {
            SearchOfferViewModel search = new SearchOfferViewModel();
            search.Page = page ?? 1;
            search.Sectors = sectors;
            search.Town = town;
            search.Word = word;
            search.IsFullTime = isFullTime.MapBools(isPartTime);
            search.IsPermanent = isPermanent.MapBools(isTemporary);
            IEnumerable<SearchResultOfferViewModel> offers = this.GetResults(search);

            this.TempData["Town"] = search.Town;
            this.TempData["Word"] = search.Word;
            this.TempData["Sectors"] = sectors;
            this.TempData["IsFullTime"] = isFullTime;
            this.TempData["IsPermanent"] = isPermanent;

            return this.View(offers.ToPagedList((int)search.Page, OffersPerPage));
        }

        private IEnumerable<JobOffer> FilterOffers(SearchOfferViewModel model)
        {
            IEnumerable<JobOffer> offers = this.FilterBySector(model.Sectors);
            if (offers != null && model.Town != null)
            {
                offers = offers.Where(o => o.TownId == model.Town);                
            }

            if (offers != null && model.Word != null)
            {
                string word = model.Word.ToLower();
                offers = offers.Where(o => o.Title.ToLower().Contains(word) || o.Description.ToLower().Contains(word) 
                    || o.BusinessSector.Name.ToLower().Contains(word) || o.Company.CompanyName.ToLower().Contains(word));
            }

            if (offers != null && model.IsFullTime != null)
            {
                offers = offers.Where(o => o.IsFullTime == model.IsFullTime);                
            }

            if (offers != null && model.IsPermanent != null)
            {
                offers = offers.Where(o => o.IsPermanent == model.IsPermanent);                
            }

            return offers;
        }

        private IEnumerable<JobOffer> FilterBySector(int[] sectorsIds)
        {
            if (sectorsIds == null || sectorsIds.Length == 0)
            {
                return this.Data.JobOffers.All().Where(o => o.IsActive);
            }

            int id = sectorsIds[0];
            IQueryable<JobOffer> result = this.Data.JobOffers.All().Where(o => o.BusinessSectorId == id && o.IsActive);

            IQueryable<JobOffer> singleSector = null;

            for (int i = 1; i < sectorsIds.Length; i++)
            {
                id = sectorsIds[i];
                singleSector = this.Data.JobOffers.All().Where(o => o.BusinessSectorId == id && o.IsActive);
                if (result != null)
                {
                    result.Concat(singleSector);
                }
                else
                {
                    result = singleSector;
                }
            }

            return result;
        }

        private IEnumerable<SearchResultOfferViewModel> GetResults(SearchOfferViewModel lastSearch)
        {
            int offersCount = 0;

            IEnumerable<SearchResultOfferViewModel> offers = this.FilterOffers(lastSearch).AsQueryable().Include("Company").Include("Town")
                .Select(SearchResultOfferViewModel.FromJobOffer).OrderByDescending(o => o.DateCreated);

            offersCount = offers.Count();

            this.TempData["OffersCount"] = offersCount;
            return offers;
        }
    }
}