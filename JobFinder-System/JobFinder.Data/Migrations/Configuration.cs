namespace JobFinder.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using JobFinder.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<JobFinderDbContext>
    {
        private static Random rnd = new Random();

        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(JobFinderDbContext context)
        {
            
            // Add roles
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);

                if (!context.Roles.Any(r => r.Name == "Company"))
                {
                    var companyRole = new IdentityRole { Name = "Company" };
                    manager.Create(companyRole);
                }

                if (!context.Roles.Any(r => r.Name == "Person"))
                {
                    var personRole = new IdentityRole { Name = "Person" };
                    manager.Create(personRole);
                }
            }

            // Add towns
            var jobOffersPerTown = new List<JobOfferPerCriteria>
            {
                new JobOfferPerCriteria
                {
                    Name = "Blagoevgrad"
                },
                new JobOfferPerCriteria
                {
                    Name = "Burgas"
                },
                new JobOfferPerCriteria
                {
                    Name = "Dobrich"
                },
                new JobOfferPerCriteria
                {
                    Name = "Gabrovo"
                },
                new JobOfferPerCriteria
                {
                    Name = "Haskovo"
                },
                new JobOfferPerCriteria
                {
                    Name = "Kardzhali"
                },
                new JobOfferPerCriteria
                {
                    Name = "Kyustendil"
                },
                new JobOfferPerCriteria
                {
                    Name = "Lovech"
                },
                new JobOfferPerCriteria
                {
                    Name = "Montana"
                },
                new JobOfferPerCriteria
                {
                    Name = "Pazardzhik"
                },
                new JobOfferPerCriteria
                {
                    Name = "Pernik"
                },
                new JobOfferPerCriteria
                {
                    Name = "Pleven"
                },
                new JobOfferPerCriteria
                {
                    Name = "Plovdiv"
                },
                new JobOfferPerCriteria
                {
                    Name = "Razgrad"
                },
                new JobOfferPerCriteria
                {
                    Name = "Ruse"
                },
                new JobOfferPerCriteria
                {
                    Name = "Shumen"
                },
                new JobOfferPerCriteria
                {
                    Name = "Silistra"
                },
                new JobOfferPerCriteria
                {
                    Name = "Sliven"
                },
                new JobOfferPerCriteria
                {
                    Name = "Smolyan"
                },
                new JobOfferPerCriteria
                {
                    Name = "Sofia City"
                },
                new JobOfferPerCriteria
                {
                    Name = "Sofia (province)"
                },
                new JobOfferPerCriteria
                {
                    Name = "Stara Zagora"
                },
                new JobOfferPerCriteria
                {
                    Name = "Targovishte"
                },
                new JobOfferPerCriteria
                {
                    Name = "Varna"
                },
                new JobOfferPerCriteria
                {
                    Name = "Veliko Tarnovo"
                },
                new JobOfferPerCriteria
                {
                    Name = "Vidin"
                },
                new JobOfferPerCriteria
                {
                    Name = "Vratsa"
                },
                new JobOfferPerCriteria
                {
                    Name = "Yambol"
                }
            };

            var towns = jobOffersPerTown.Select(t => new Town { Name = t.Name }).ToArray();

            if (!context.Towns.Any())
            {
                context.Towns.AddOrUpdate(
                    t => t.Name,
                    towns);
            }

            // Add business sectors
            var jobOffersPerBusinessSector = new List<JobOfferPerCriteria>
            {
                new JobOfferPerCriteria
                {
                    Name = "Accounting, Audit, Finance",
                    Count = 1272
                },
                new JobOfferPerCriteria
                {
                    Name = "Administrative and office",
                    Count = 2462
                },
                new JobOfferPerCriteria
                {
                    Name = "Advertising, PR",
                    Count = 353
                },
                new JobOfferPerCriteria
                {
                    Name = "Agricultures, Forestry & Fishing",
                    Count = 244
                },
                new JobOfferPerCriteria
                {
                    Name = "Architecture, Construction",
                    Count = 1540
                },
                new JobOfferPerCriteria
                {
                    Name = "Arts, Entertainment, Promotion",
                    Count = 588
                },
                new JobOfferPerCriteria
                {
                    Name = "Automotive, Auto Service, Gas Stations",
                    Count = 1307
                },
                new JobOfferPerCriteria
                {
                    Name = "Aviation, Airport & Airline",
                    Count = 55
                },
                new JobOfferPerCriteria
                {
                    Name = "Banking, Lending",
                    Count = 892
                },
                new JobOfferPerCriteria
                {
                    Name = "Beauty, SPA & Salon",
                    Count = 492
                },
                new JobOfferPerCriteria
                {
                    Name = "Business/Consultancy services",
                    Count = 765
                },
                new JobOfferPerCriteria
                {
                    Name = "Cleaning, Household services",
                    Count = 1062
                },
                new JobOfferPerCriteria
                {
                    Name = "Contact centers (Call centers)",
                    Count = 1080
                },
                new JobOfferPerCriteria
                {
                    Name = "Design, Creative, Video & Animation",
                    Count = 360
                },
                new JobOfferPerCriteria
                {
                    Name = "Drivers, Couriers",
                    Count = 1828
                },
                new JobOfferPerCriteria
                {
                    Name = "Education, Courses, Translators",
                    Count = 594
                },
                new JobOfferPerCriteria
                {
                    Name = "Energy and Utilities",
                    Count = 429
                },
                new JobOfferPerCriteria
                {
                    Name = "Engineers",
                    Count = 1278
                },
                new JobOfferPerCriteria
                {
                    Name = "Healthcare and pharmacy",
                    Count = 1041
                },
                new JobOfferPerCriteria
                {
                    Name = "Hotels",
                    Count = 1523
                },
                new JobOfferPerCriteria
                {
                    Name = "Human Resources",
                    Count = 282
                },
                new JobOfferPerCriteria
                {
                    Name = "Installation, Maintenance and Repair",
                    Count = 1452
                },
                new JobOfferPerCriteria
                {
                    Name = "Insurance",
                    Count = 333
                },
                new JobOfferPerCriteria
                {
                    Name = "IT - Administration and sales",
                    Count = 856
                },
                new JobOfferPerCriteria
                {
                    Name = "IT - Hardware design/maintenance",
                    Count = 563
                },
                new JobOfferPerCriteria
                {
                    Name = "IT - Software development/maintenance",
                    Count = 1892
                },
                new JobOfferPerCriteria
                {
                    Name = "Legal",
                    Count = 122
                },
                new JobOfferPerCriteria
                {
                    Name = "Management, Business development",
                    Count = 492
                },
                new JobOfferPerCriteria
                {
                    Name = "Manual work",
                    Count = 804
                },
                new JobOfferPerCriteria
                {
                    Name = "Manufacturing - Electronics, Electrical, Mechanical",
                    Count = 1115
                },
                new JobOfferPerCriteria
                {
                    Name = "Manufacturing - Chemistry and Oil",
                    Count = 120
                },
                new JobOfferPerCriteria
                {
                    Name = "Manufacturing - Food and Beverage",
                    Count = 1111
                },
                new JobOfferPerCriteria
                {
                    Name = "Manufacturing - Furniture and Carpentry",
                    Count = 197
                },
                new JobOfferPerCriteria
                {
                    Name = "Manufacturing - Pharmaceutical",
                    Count = 70
                },
                new JobOfferPerCriteria
                {
                    Name = "Manufacturing - Textile and Clothes",
                    Count = 580
                },
                new JobOfferPerCriteria
                {
                    Name = "Manufacturing - Ìetallurgy and Mining",
                    Count = 187
                },
                new JobOfferPerCriteria
                {
                    Name = "Manufacturing - Other",
                    Count = 2056
                },
                new JobOfferPerCriteria
                {
                    Name = "Marine and Shipping",
                    Count = 37
                },
                new JobOfferPerCriteria
                {
                    Name = "Marketing",
                    Count = 831
                },
                new JobOfferPerCriteria
                {
                    Name = "Media, Publishing",
                    Count = 170
                },
                new JobOfferPerCriteria
                {
                    Name = "NGO",
                    Count = 60
                },
                new JobOfferPerCriteria
                {
                    Name = "Public sector, Government",
                    Count = 193
                },
                new JobOfferPerCriteria
                {
                    Name = "Real-estate",
                    Count = 276
                },
                new JobOfferPerCriteria
                {
                    Name = "Research and development",
                    Count = 174
                },
                new JobOfferPerCriteria
                {
                    Name = "Restaurants and Catering",
                    Count = 3420
                },
                new JobOfferPerCriteria
                {
                    Name = "Retail, Wholesale",
                    Count = 6926
                },
                new JobOfferPerCriteria
                {
                    Name = "Security",
                    Count = 511
                },
                new JobOfferPerCriteria
                {
                    Name = "Sports, Kinesiotherapy",
                    Count = 210
                },
                new JobOfferPerCriteria
                {
                    Name = "Telecommunications - Administration and sales",
                    Count = 447
                },
                new JobOfferPerCriteria
                {
                    Name = "Telecommunications - engineers and technicians",
                    Count = 327
                },
                new JobOfferPerCriteria
                {
                    Name = "Transport, Logistics, Spedition",
                    Count = 1822
                },
                new JobOfferPerCriteria
                {
                    Name = "Travel and Tourism",
                    Count = 261
                }
            };

            var businessSectors = jobOffersPerBusinessSector.Select(s => new BusinessSector { Name = s.Name }).ToArray();

            if (!context.BusinessSectors.Any())
            {
                context.BusinessSectors.AddOrUpdate(
                    s => s.Name,
                    businessSectors);
            }

            // Create admin, company and person
            var companies = new List<Company>();

            if (!context.Users.Any(u => u.UserName == "admin@admin.bg"))
            {
                var store = new UserStore<User>(context);
                var manager = new UserManager<User>(store);

                var user = new Person
                {
                    UserName = "admin@admin.bg",
                    Email = "admin@admin.bg",
                    FirstName = "Admin",
                    LastName = "Admin"
                };
                manager.Create(user, "123123");
                manager.AddToRole(user.Id, "Admin");

                if (!context.Companies.Any(c => c.Bulstat == "1234567891234"))
                {
                    var company = new Company
                    {
                        Bulstat = "1234567891234",
                        Email = "aaaaaa@abv.bg",
                        BusinessSectors = new HashSet<BusinessSector>(businessSectors.Take(2)),
                        UserName = "aaaaaa@abv.bg",
                        CompanyName = "Abc",
                        IsApproved = true,
                        PhoneNumber = "0888888888"
                    };

                    companies.Add(company);
                    manager.Create(company, "123123");
                    manager.AddToRole(company.Id, "Company");
                }

                if (!context.Companies.Any(c => c.Bulstat == "2234567891234"))
                {
                    var company = new Company
                    {
                        Bulstat = "2234567891234",
                        Email = "company@abv.bg",
                        BusinessSectors = new HashSet<BusinessSector>(businessSectors.Skip(2).Take(1)),
                        UserName = "company@abv.bg",
                        CompanyName = "B&B",
                        IsApproved = true,
                        PhoneNumber = "0888888666"
                    };

                    companies.Add(company);
                    manager.Create(company, "123123");
                    manager.AddToRole(company.Id, "Company");
                }

                if (!context.People.Any(p => p.Email == "person@abv.bg"))
                {
                    var person = new Person
                    {
                        Email = "person@abv.bg",
                        FirstName = "Petar",
                        LastName = "Petrov",
                        UserName = "person@abv.bg"
                    };

                    manager.Create(person, "123123");
                    manager.AddToRole(person.Id, "Person");
                }
            }

            // Create job offers
            if (!context.JobOffers.Any())
            {
                context = this.RecreateContext(context);

                foreach (var sector in jobOffersPerBusinessSector)
                {
                    for (int i = 0; i < sector.Count; i++)
                    {
                        context.JobOffers.Add(new JobOffer
                        {
                            Title = string.Format("{0} {1}", sector.Name, i),
                            Description = string.Format(
                                "Amazing job from sector: {0}, which is {1} of {2} offers in this sector",
                                sector.Name,
                                i,
                                sector.Count),
                            DateCreated = this.GetRandomDate(),
                            IsActive = true,
                            CompanyId = companies[rnd.Next(companies.Count())].Id,
                            TownId = towns[rnd.Next(towns.Count())].Id,
                            BusinessSectorId = businessSectors.Single(s => s.Name == sector.Name).Id
                        });

                        if ((i + 1) % 100 == 0)
                        {
                            context = this.RecreateContext(context);
                        }
                    }

                    context = this.RecreateContext(context);
                }
            }

            context.Configuration.AutoDetectChangesEnabled = true;
            context.Configuration.ValidateOnSaveEnabled = true;
            
        }

        private JobFinderDbContext RecreateContext(JobFinderDbContext context)
        {
            context.SaveChanges();
            context.Dispose();
            context = JobFinderDbContext.Create();
            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;

            return context;
        }

        private DateTime GetRandomDate()
        {
            var start = new DateTime(2015, 1, 1);
            var range = (DateTime.Today - start).Days;

            return start.AddDays(rnd.Next(range));
        }

        private class JobOfferPerCriteria
        {
            public string Name { get; set; }

            public int Count { get; set; }
        }
    }
}
