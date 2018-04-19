namespace JobFinder.Web.Models.DashboardModels
{
    using System.Collections.Generic;

    public class DashboardColumnViewModel
    {
        public DashboardColumnViewModel()
        {
            this.Categories = new HashSet<string>();
            this.Series = new HashSet<ColumnViewModel>();
        }

        public ICollection<string> Categories { get; set; }

        public ICollection<ColumnViewModel> Series { get; set; }
    }
}
