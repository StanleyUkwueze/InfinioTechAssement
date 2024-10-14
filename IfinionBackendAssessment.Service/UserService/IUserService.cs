using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;

namespace IfinionBackendAssessment.Service.UserService
{
    public interface IUserService
    {
        Task<APIResponse<UserResponse>> GetUserByEmail(string email);
        Task<APIResponse<UserResponse>> Login(LoginRequest request);
        Task<APIResponse<CreatedUserResponse>> CreateUser(CreateUserRequest createUserRequest);
    }
}
