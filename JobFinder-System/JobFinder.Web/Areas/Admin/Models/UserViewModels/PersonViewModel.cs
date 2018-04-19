namespace JobFinder.Web.Areas.Admin.Models.UserViewModels
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;

    public class PersonViewModel
    {
        public static Expression<Func<JobFinder.Models.Person, PersonViewModel>> FromPerson
        {
            get
            {
                return c => new PersonViewModel
                {
                    Id = c.Id,
                    Email = c.Email,
                    FirstName = c.FirstName,
                    LastName = c.LastName
                };
            }
        }

        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
