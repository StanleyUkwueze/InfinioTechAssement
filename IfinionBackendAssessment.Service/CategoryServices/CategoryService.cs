using AutoMapper;
using IfinionBackendAssessment.DataAccess.CategoryRepository;
using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;

namespace IfinionBackendAssessment.Service.CategoryServices
{
    public class CategoryService(ICategoryRepo categoryRepo, IMapper mapper): ICategoryService
    {
        public async Task<APIResponse<AddCategoryResponse>> AddCategory(AddCategoryDto categoryDto)
        {
            APIResponse<AddCategoryResponse> response = new APIResponse<AddCategoryResponse>();

            if (categoryDto == null)
                return new APIResponse<AddCategoryResponse> { IsSuccessful = false, Message = "Please, provide the category you want to add" };

            var existingCategory = await categoryRepo.GetCategoryByName(categoryDto.Name);
            if (existingCategory is not null) return new APIResponse<AddCategoryResponse> { IsSuccessful = false, Message = "Category already taken" };

            var catToAdd = mapper.Map<AddCategoryDto, Category>(categoryDto);
            catToAdd.DateUpdated = DateTime.Now;

            var isAdded = await categoryRepo.AddAsync(catToAdd);
            if (isAdded)
            {
                response.Message = "Category added successfully";
                response.IsSuccessful = true;
                response.Data = mapper.Map<Category, AddCategoryResponse>(catToAdd);
                return response;
            }

            response.Message = "Category addition failed";
            response.IsSuccessful = false;
            return response;

        }

        public async Task<APIResponse<CategoryResponseDto>> UpdateCategory(UpdateCategoryDto updateCategoryDto, int id)
        {
            if (id <= 0) return new APIResponse<CategoryResponseDto>
            {
                Message = "Kindly supply a valid category Id",
                IsSuccessful = false
            };

            var updatedProduct = await categoryRepo.Update(updateCategoryDto, id)!;

            if (id > 0)
            {
                var ProductToreturn = mapper.Map<CategoryResponseDto>(updatedProduct);
                return new APIResponse<CategoryResponseDto>
                {
                    Message = "Category successfully updated",
                    IsSuccessful = true,
                    Data = ProductToreturn
                };
            }
            return new APIResponse<CategoryResponseDto>
            {
                Message = "Category update failed",
                IsSuccessful = false,
                Errors = new string[] { "Product Update Failed" }
            };

        }

        public async Task<APIResponse<string>> DeleteCategory(int Id)
        {
            if (Id <= 0) return new APIResponse<string>
            {
                Message = "Kindly supply a valid category Id",
                IsSuccessful = false
            };

            var catToDelete = categoryRepo.GetFirstOrDefauly(x => x.Id == Id);
            if (catToDelete is null) return new APIResponse<string> { Message = " Category does not exist", IsSuccessful = false };
            var isRemoved = await categoryRepo.RemoveAsync(catToDelete);
            if (isRemoved) return new APIResponse<string> { Message = "Category successfully deleted", IsSuccessful = true };

            return new APIResponse<string> { Message = "Category deletion failed", IsSuccessful = false };
        }
    }


}
