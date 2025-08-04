using CBS.BankMGT.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Domain.Context
{
    public static class DbInitializer
    {
        public static void Initialize(POSContext context)
        {
            try
            {
                // Ensure the database is created
                context.Database.EnsureCreated();

                // Seed Country if empty
                SeedCountry(context);

                // Seed Region if empty
                SeedRegions(context);

                // Seed Division if empty
                SeedDivisions(context);

                // Seed Subdivision if empty
                SeedSubdivisions(context);

                // Seed Town if empty
                SeedTowns(context);

                // Seed Organization if empty
                SeedOrganization(context);

                // Seed Banks if empty
                SeedBanks(context);

                // Seed Branches if empty
                SeedBranches(context);

            }
            catch (Exception ex)
            {
                // Log or handle the exception here
                throw new Exception("Database initialization failed", ex);
            }
        }

        // Seed Country
        private static void SeedCountry(POSContext context)
        {
            if (!context.Countries.Any())
            {
                Country defaultCountry = new Country
                {
                    Id = "1",
                    Code = "CM",
                    Name = "Cameroon",
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                };

                context.Countries.Add(defaultCountry);
                context.SaveChanges();
            }
        }

        // Seed Region
        private static void SeedRegions(POSContext context)
        {
            if (!context.Regions.Any())
            {
                var defaultCountry = context.Countries.FirstOrDefault(c => c.Code == "237");

                if (defaultCountry != null)
                {
                    var regions = new List<Region>
            {
                new Region
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Adamawa",
                    CountryId = defaultCountry.Id,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                },
                new Region
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Centre",
                    CountryId = defaultCountry.Id,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                },
                new Region
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "East",
                    CountryId = defaultCountry.Id,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                },
                new Region
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Far North",
                    CountryId = defaultCountry.Id,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                },
                new Region
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Littoral",
                    CountryId = defaultCountry.Id,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                },
                new Region
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "North",
                    CountryId = defaultCountry.Id,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                },
                new Region
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Northwest",
                    CountryId = defaultCountry.Id,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                },
                new Region
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "West",
                    CountryId = defaultCountry.Id,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                },
                new Region
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "South",
                    CountryId = defaultCountry.Id,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                },
                new Region
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Southwest",
                    CountryId = defaultCountry.Id,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                }
            };

                    context.Regions.AddRange(regions);
                    context.SaveChanges();
                }
            }
        }

        private static void SeedDivisions(POSContext context)
        {
            if (!context.Divisions.Any())
            {
                var regions = context.Regions.ToList(); // Fetch all regions

                var divisions = new List<Division>
        {
            // Adamawa Region Divisions
            new Division
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Ngaoundere",
                RegionId = regions.FirstOrDefault(r => r.Name == "Adamawa")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Centre Region Divisions
            new Division
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Yaounde",
                RegionId = regions.FirstOrDefault(r => r.Name == "Centre")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // East Region Divisions
            new Division
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Bertoua",
                RegionId = regions.FirstOrDefault(r => r.Name == "East")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Far North Region Divisions
            new Division
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Maroua",
                RegionId = regions.FirstOrDefault(r => r.Name == "Far North")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Littoral Region Divisions
            new Division
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Douala",
                RegionId = regions.FirstOrDefault(r => r.Name == "Littoral")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // North Region Divisions
            new Division
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Garoua",
                RegionId = regions.FirstOrDefault(r => r.Name == "North")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Northwest Region Divisions
            new Division
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Bamenda",
                RegionId = regions.FirstOrDefault(r => r.Name == "Northwest")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // West Region Divisions
            new Division
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Bafoussam",
                RegionId = regions.FirstOrDefault(r => r.Name == "West")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // South Region Divisions
            new Division
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Ebolowa",
                RegionId = regions.FirstOrDefault(r => r.Name == "South")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Southwest Region Divisions
            new Division
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Buea",
                RegionId = regions.FirstOrDefault(r => r.Name == "Southwest")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            }
        };

                context.Divisions.AddRange(divisions);
                context.SaveChanges();
            }
        }

        private static void SeedSubdivisions(POSContext context)
        {
            if (!context.Subdivisions.Any())
            {
                var divisions = context.Divisions.ToList(); // Fetch all divisions

                var subdivisions = new List<Subdivision>
        {
            // Ngaoundere Division Subdivisions
            new Subdivision
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Ngaoundere I",
                DivisionId = divisions.FirstOrDefault(d => d.Name == "Ngaoundere")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Yaounde Division Subdivisions
            new Subdivision
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Yaounde I",
                DivisionId = divisions.FirstOrDefault(d => d.Name == "Yaounde")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Bertoua Division Subdivisions
            new Subdivision
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Bertoua I",
                DivisionId = divisions.FirstOrDefault(d => d.Name == "Bertoua")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Maroua Division Subdivisions
            new Subdivision
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Maroua I",
                DivisionId = divisions.FirstOrDefault(d => d.Name == "Maroua")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Douala Division Subdivisions
            new Subdivision
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Douala I",
                DivisionId = divisions.FirstOrDefault(d => d.Name == "Douala")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Garoua Division Subdivisions
            new Subdivision
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Garoua I",
                DivisionId = divisions.FirstOrDefault(d => d.Name == "Garoua")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Bamenda Division Subdivisions
            new Subdivision
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Bamenda I",
                DivisionId = divisions.FirstOrDefault(d => d.Name == "Bamenda")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Bafoussam Division Subdivisions
            new Subdivision
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Bafoussam I",
                DivisionId = divisions.FirstOrDefault(d => d.Name == "Bafoussam")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Ebolowa Division Subdivisions
            new Subdivision
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Ebolowa I",
                DivisionId = divisions.FirstOrDefault(d => d.Name == "Ebolowa")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Buea Division Subdivisions
            new Subdivision
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Buea I",
                DivisionId = divisions.FirstOrDefault(d => d.Name == "Buea")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            }
        };

                context.Subdivisions.AddRange(subdivisions);
                context.SaveChanges();
            }
        }

        // Seed Town
        private static void SeedTowns(POSContext context)
        {
            if (!context.Towns.Any())
            {
                var subdivisions = context.Subdivisions.ToList(); // Fetch all subdivisions

                var towns = new List<Town>
        {
            // Ngaoundere I Subdivision Towns
            new Town
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Ngaoundere Town",
                SubdivisionId = subdivisions.FirstOrDefault(s => s.Name == "Ngaoundere I")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Yaounde I Subdivision Towns
            new Town
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Yaounde Town",
                SubdivisionId = subdivisions.FirstOrDefault(s => s.Name == "Yaounde I")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Bertoua I Subdivision Towns
            new Town
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Bertoua Town",
                SubdivisionId = subdivisions.FirstOrDefault(s => s.Name == "Bertoua I")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Maroua I Subdivision Towns
            new Town
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Maroua Town",
                SubdivisionId = subdivisions.FirstOrDefault(s => s.Name == "Maroua I")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Douala I Subdivision Towns
            new Town
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Douala Town",
                SubdivisionId = subdivisions.FirstOrDefault(s => s.Name == "Douala I")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Garoua I Subdivision Towns
            new Town
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Garoua Town",
                SubdivisionId = subdivisions.FirstOrDefault(s => s.Name == "Garoua I")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Bamenda I Subdivision Towns
            new Town
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Bamenda Town",
                SubdivisionId = subdivisions.FirstOrDefault(s => s.Name == "Bamenda I")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Bafoussam I Subdivision Towns
            new Town
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Bafoussam Town",
                SubdivisionId = subdivisions.FirstOrDefault(s => s.Name == "Bafoussam I")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Ebolowa I Subdivision Towns
            new Town
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Ebolowa Town",
                SubdivisionId = subdivisions.FirstOrDefault(s => s.Name == "Ebolowa I")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            },
            // Buea I Subdivision Towns
            new Town
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Buea Town",
                SubdivisionId = subdivisions.FirstOrDefault(s => s.Name == "Buea I")?.Id,
                CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                CreatedDate = DateTime.Now,
                DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                IsDeleted = false,
                ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                DeletedDate = DateTime.MinValue,
                ModifiedDate = DateTime.MinValue
            }
        };

                context.Towns.AddRange(towns);
                context.SaveChanges();
            }
        }

        // Seed Organization
        private static void SeedOrganization(POSContext context)
        {
            if (!context.Organizations.Any())
            {
                Organization defaultOrganization = new Organization
                {
                    Id = "1",
                    Name = "Default Organization",
                    Description = "Default Organization Description",
                    CountryId = "1",
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                };

                context.Organizations.Add(defaultOrganization);
                context.SaveChanges();
            }
        }

        // Seed Banks
        private static void SeedBanks(POSContext context)
        {
            if (!context.Banks.Any())
            {
                Organization defaultOrganization = context.Organizations.FirstOrDefault(); // Assuming at least one organization exists

                Bank defaultBank = new Bank
                {
                    Id = "1",
                    BankCode = "012",
                    Name = "Default Bank",
                    Description = "Default Bank Description",
                    Telephone = "1234567890",
                    Email = "defaultbank@example.com",
                    Address = "123 Default Street",
                    Capital = "$1000000",
                    RegistrationNumber = "123456789",
                    LogoUrl = "https://example.com/defaultbanklogo",
                    ImmatriculationNumber = "IM123",
                    TaxPayerNUmber = "TAX123",
                    PBox = "PO Box 123",
                    WebSite = "https://www.defaultbank.com",
                    DateOfCreation = "01/01/2000",
                    BankInitial = "DB",
                    Motto = "Serving you better",
                    OrganizationId = defaultOrganization.Id,
                    Organization = defaultOrganization,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue, CustomerServiceContact="8080",
                };

                context.Banks.Add(defaultBank);
                context.SaveChanges();
            }
        }

        // Seed Branches
        private static void SeedBranches(POSContext context)
        {
            if (!context.Branches.Any())
            {
                Bank defaultBank = context.Banks.FirstOrDefault(); // Assuming at least one bank exists

                Branch defaultBranch = new Branch
                {
                    Id = "1",
                    BranchCode = "000",
                    Name = "Default Branch",
                    Location = "Default Location",
                    Telephone = "9876543210",
                    Email = "defaultbranch@example.com",
                    Address = "456 Default Street",
                    BankId = defaultBank.Id, // Assuming Bank Id is provided
                    Capital = "$500000",
                    RegistrationNumber = "987654321",
                    LogoUrl = "https://example.com/defaultbranchlogo",
                    ImmatriculationNumber = "IM456",
                    TaxPayerNUmber = "TAX456",
                    PBox = "PO Box 456",
                    WebSite = "https://www.defaultbranch.com",
                    DateOfCreation = "01/01/2005",
                    BankInitial = "DB",
                    Motto = "Branching out for you",
                    HeadOfficeTelehoneNumber = "1234567890",
                    HeadOfficeAddress = "123 Default Street",
                    ActiveStatus = true,
                    IsHeadOffice = true,
                    CreatedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    CreatedDate = DateTime.Now,
                    DeletedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    IsDeleted = false,
                    ModifiedBy = "4B352B37-332A-40C6-AB05-E38FCF109719",
                    DeletedDate = DateTime.MinValue,
                    ModifiedDate = DateTime.MinValue
                };

                context.Branches.Add(defaultBranch);
                context.SaveChanges();
            }
        }
    }

}
