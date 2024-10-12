using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.DataAccess.CategoryRepository
{
    public interface ICategoryRepo
    {
        Task<Category> Update(UpdateCategoryDto category, string? image);
        Task<Category?> GetCategoryByName(string cateName);
        IQueryable<Category> GetAllCategories();
    }
}
