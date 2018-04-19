namespace JobFinder.Web.Areas.Company.Models.BussinessCardViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class EditBussinessCardViewModel
    {
        public string Id { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public int[] BusinessSectors { get; set; }

        public string Address { get; set; }

        public string WebSite { get; set; }

        public string AboutUs { get; set; }
    }
}
