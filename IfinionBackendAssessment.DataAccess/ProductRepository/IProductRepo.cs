using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;

namespace IfinionBackendAssessment.DataAccess.ProductRepository
{
    public interface IProductRepo : IGenericRepository<Product>
    {
        Task<Product> Update(UpdateProductDto product, int id, string? image);
        Task<Product> GetById(int id);
        IQueryable<Product> GetAllProducts();
        IQueryable<Product> GetProductsWithSearch(string? searchTerm, decimal minPrice, decimal maxPrice);
    }
}
