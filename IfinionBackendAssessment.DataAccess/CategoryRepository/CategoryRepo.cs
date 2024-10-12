using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace IfinionBackendAssessment.DataAccess.CategoryRepository
{
    public class CategoryRepo : GenericRepository<Category>, ICategoryRepo
    {
        private readonly AppDbContext _context;
        public CategoryRepo(AppDbContext context): base(context) 
        {
            _context = context;
        }

        public IQueryable<Category> GetAllCategories()
        {
            return _context.Categories.Include(x => x.Products).AsQueryable();
        }

        public async Task<Category?> GetCategoryByName(string cateName)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Name == cateName);
        }

        public async Task<Category> Update(UpdateCategoryDto category, string? image)
        {
            var categoryToUpdate = await _context.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
            var isSaved = 0;

            if (categoryToUpdate is not null)
            {
                categoryToUpdate.Name = category.Name is not null ? category.Name! : categoryToUpdate.Name;
                categoryToUpdate.Description = category.Description is not null ? category.Description! : categoryToUpdate.Description;
                categoryToUpdate.DateUpdated = DateTime.UtcNow;

                _context.Categories.Update(categoryToUpdate);
                isSaved = await _context.SaveChangesAsync();
            }

            return isSaved > 0 ? categoryToUpdate! : new Category { };
        }
    }
}
