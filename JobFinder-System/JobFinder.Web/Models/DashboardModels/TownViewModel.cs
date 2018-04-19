namespace JobFinder.Web.Models.DashboardModels
{
    using System.Collections.Generic;

    public class TownViewModel
    {
        public string Name { get; set; }

        public IEnumerable<OfferViewModel> Offers { get; set; }
    }
}
