using AutoMapper;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.Common
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserResponse>().ReverseMap();
            CreateMap<AddProductDto,Product>().ReverseMap();
            CreateMap<Product, ProductResponseDto>().ReverseMap();
            CreateMap<User, CreatedUserResponse>().ReverseMap();
            CreateMap<Category, CategoryResponseDto>().ReverseMap();
            CreateMap<AddCategoryDto, Category>().ReverseMap();
            CreateMap<AddCategoryResponse, Category>().ReverseMap();
        }
    }
}
