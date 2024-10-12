using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;

namespace IfinionBackendAssessment.Service.UserService
{
    public interface IUserService
    {
        Task<ApiResponse<UserResponse>> GetUserByEmail(string email);
        Task<ApiResponse<UserResponse>> Login(LoginRequest request);
        Task<ApiResponse<CreatedUserResponse>> CreateUser(CreateUserRequest createUserRequest);
    }
}
