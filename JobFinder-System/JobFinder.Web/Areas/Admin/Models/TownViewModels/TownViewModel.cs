namespace JobFinder.Web.Areas.Admin.Models.TownViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using JobFinder.Models;

    public class TownViewModel
    {
        public static Expression<Func<Town, TownViewModel>> FromTown
        {
            get
            {
                return t => new TownViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                };
            }
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [MaxLength(20)]
        public string Name { get; set; }
    }
}
