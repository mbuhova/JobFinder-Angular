namespace JobFinder.Web.Models.DashboardModels
{
    using System.Collections.Generic;

    public class ColumnViewModel
    {
        public ColumnViewModel()
        {
            this.Data = new List<int>();
        }

        public string Name { get; set; }

        public ICollection<int> Data { get; set; }
    }
}
