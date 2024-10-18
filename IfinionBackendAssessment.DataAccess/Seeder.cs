using IfinionBackendAssessment.Entity.Entities;

namespace IfinionBackendAssessment.DataAccess
{
    public static class Seeder
    {
        public async static Task SeedDatabase(AppDbContext context)
        {
            var users = new List<User>();
            var password = "P@ssW0rd";
            var role = "Customer";

            if (!context.Users.Any())
            {
                users = new List<User> {
                    new User
                    {
                        UserName = "Admin",
                        Email = "admin@gmail.com",
                        PaswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                        Role = "Admin",
                    },
                   new User
                   {
                    UserName = "Peter Ayo",
                    Email = "peterayo@gmail.com",
                    PaswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = role,
                   },
                    new User
                   {
                    UserName = "Faith Riwan",
                    Email = "Riwan@gmail.com",
                    PaswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = role,
                   },
                     new User
                   {
                    UserName = "Emeka Garba",
                    Email = "Emeka@gmail.com",
                    PaswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = role,
                   }
               }; 
                
             context.Users.AddRange(users);
            await context.SaveChangesAsync();
            }

            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category
                    {
                        Name = "Groceries",
                        Description = "All kinds of food items",
                        Products = new List<Product>
                        {
                            new Product
                            {
                                Name = "Indomie",
                                ImageUrl = "https://www.testurl.com",
                                CategoryName = "Groceries",
                                Description = "Indomie for all",
                                InStock = true,
                                Count = 12,
                                Price = 222
                            },
                            new Product
                            {
                                Name = "Amala",
                                ImageUrl = "https://www.testurl.com",
                                CategoryName = "Groceries",
                                Description = "Amala for all",
                                InStock = true,
                                Count = 35,
                                Price = 344
                            }
                        }
                    },
                     new Category
                    {
                        Name = "Men's Wear",
                        Description = "All kinds of Men's Wear",
                         Products = new List<Product>
                        {
                            new Product
                            {
                                Name = "Jeans",
                                ImageUrl = "https://www.testurl.com",
                                CategoryName = "Men's Wear",
                                Description = "Jeans for all",
                                InStock = true,
                                Count = 12,
                                Price = 222
                            },
                            new Product
                            {
                                Name = "Polo Shirt",
                                ImageUrl = "https://www.testurl.com",
                                CategoryName = "Men's Wear",
                                Description = "Polo Shirt for all",
                                InStock = true,
                                Count = 45,
                                Price = 2345
                            }
                        }
                    },
                      new Category
                    {
                        Name = "Fruits",
                        Description = "All kinds of Fruits",
                         Products = new List<Product>
                        {
                            new Product
                            {
                                Name = "Orange",
                                ImageUrl = "https://www.testurl.com",
                                CategoryName = "Fruits",
                                Description = "Orange for all",
                                InStock = true,
                                Count = 12,
                                Price = 344
                            },
                            new Product
                            {
                                Name = "Banana",
                                ImageUrl = "https://www.testurl.com",
                                CategoryName = "Fruits",
                                Description = "Banana for all",
                                InStock = true,
                                Count = 35,
                                Price = 344
                            }
                        }
                    },
                       new Category
                    {
                        Name = "Furniture",
                        Description = "All kinds of Furniture",
                         Products = new List<Product>
                        {
                            new Product
                            {
                                Name = "Center Table",
                                ImageUrl = "https://www.testurl.com",
                                CategoryName = "Furniture",
                                Description = "Center Table for all",
                                InStock = true,
                                Count = 12,
                                Price = 232
                            },
                            new Product
                            {
                                Name = "Office Chair",
                                ImageUrl = "https://www.testurl.com",
                                CategoryName = "Furniture",
                                Description = "Office Chair for all",
                                InStock = true,
                                Count = 35,
                                Price = 344
                            }
                        }
                    },
                        new Category
                    {
                        Name = "Electronics",
                        Description = "All kinds of Electronics",
                         Products = new List<Product>
                        {
                            new Product
                            {
                                Name = "Smart Tv",
                                ImageUrl = "https://www.testurl.com",
                                CategoryName = "Electronics",
                                Description = "Smart Tv for all",
                                InStock = true,
                                Count = 232,
                                Price = 222
                            },
                            new Product
                            {
                                Name = "Mp3 Speaker",
                                ImageUrl = "https://www.testurl.com",
                                CategoryName = "Electronics",
                                Description = "Mp3 Speaker for all",
                                InStock = true,
                                Count = 35,
                                Price = 344
                            }
                        }
                    }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }
           


        }
    }
}
