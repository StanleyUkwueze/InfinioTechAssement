using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;

namespace IfinionBackendAssessment.DataAccess.CategoryRepository
{
    public interface ICategoryRepo: IGenericRepository<Category>
    {
        Task<Category> Update(UpdateCategoryDto category, int id);
        Task<Category?> GetCategoryByName(string cateName);
        IQueryable<Category> GetAllCategories();
    }
}
