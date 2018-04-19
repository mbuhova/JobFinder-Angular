namespace JobFinder.Web.Areas.Admin.Models.BusinessSectorViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using JobFinder.Models;

    public class BusinessSectorViewModel
    {
        public static Expression<Func<BusinessSector, BusinessSectorViewModel>> FromBusinessSector
        {
            get
            {
                return t => new BusinessSectorViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                };
            }
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [MaxLength(60)]
        public string Name { get; set; }
    }
}
