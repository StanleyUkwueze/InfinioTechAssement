using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.DataAccess.ProductRepository
{
    public class ProductRepo : GenericRepository<Product>, IProductRepo
    {
        private readonly AppDbContext _context;
        public ProductRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Product> GetProductsWithSearch(string? searchTerm, decimal minPrice, decimal maxPrice)
        {
            var products = _context.Products.Include(x => x.Category).AsQueryable();
            if (!products.Any()) return products;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerCaseTerm = searchTerm.Trim().ToLower();
                products = products.Where(e => e.Name.ToLower().Contains(lowerCaseTerm)
                                        || e.Category.Name.ToLower().Contains(lowerCaseTerm));
            }

            if (minPrice > 0 || maxPrice > 0)
            {
                products = products.Where(x => x.Price >= minPrice && x.Price <= maxPrice);
            }

            return products;
        }


        public async Task<Product> GetProductByName(string prodName)
        {
            if (string.IsNullOrEmpty(prodName)) return new Product { };

            var result = await _context.Products.FirstOrDefaultAsync(c => c.Name == prodName);
            if (result != null) return result;

            return new Product { };
        }

        public IQueryable<Product> GetAllProducts()
        {
            return _context.Products.Include(x => x.Category).AsQueryable();
        }

        public async Task<Product> GetById(int id)
        {
            var product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
            return product!;
        }

        public async Task<Product> Update(UpdateProductDto product, int id, string? image)
        {
            var productToUpdate = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(c => c.Id == id);
            var isSaved = 0;
            if (productToUpdate is not null)
            {
                productToUpdate.Name = !string.IsNullOrEmpty(product.Name) ? product.Name! : productToUpdate.Name;
                productToUpdate.Description = !string.IsNullOrEmpty(product.Description) ? product.Description! : productToUpdate.Description;
                productToUpdate.Price = product.Price != 0 ? product.Price : productToUpdate.Price;
                productToUpdate.ImageUrl = !string.IsNullOrEmpty(image) ? image! : productToUpdate.ImageUrl;
                productToUpdate.DateUpdated = DateTime.Now;

                _context.Products.Update(productToUpdate);
                isSaved = await _context.SaveChangesAsync();

            }
            return isSaved > 0 ? productToUpdate! : new Product { };

        }
    }
}
