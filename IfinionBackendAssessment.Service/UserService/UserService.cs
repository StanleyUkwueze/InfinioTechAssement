using AutoMapper;
using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.UserRepository;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using IfinionBackendAssessment.Service.JWT;
using IfinionBackendAssessment.Service.MailService;
using System.Text.Encodings.Web;

namespace IfinionBackendAssessment.Service.UserService
{
    public class UserService(IUserRepository userRepository,IEMailService eMailService, IJWTService jWTService, IMapper mapper) : IUserService
    {
        public async Task<APIResponse<CreatedUserResponse>> CreateUser(CreateUserRequest createUserRequest)
        {
            var response = new APIResponse<CreatedUserResponse>();
            try
            {
                var isValid = UserDataValidation(createUserRequest);
                if (!isValid.IsSuccessful) return isValid;

                var existingUser = await userRepository.GetUserByEmailOrUserName(createUserRequest.Email.Trim());
                if (existingUser is not null)
                    return new APIResponse<CreatedUserResponse>
                    {
                        IsSuccessful = false,
                        Message = "User email already exist",
                    };

                var user = new User
                {
                    UserName = createUserRequest.UserName.Trim(),
                    Email = createUserRequest.Email.Trim(),
                    PaswordHash = BCrypt.Net.BCrypt.HashPassword(createUserRequest.Password),
                    Role = "Customer",
                };

                var userCreated = await userRepository.AddAsync(user);
                if (!userCreated) return new APIResponse<CreatedUserResponse>
                {
                    IsSuccessful = false,
                    Message = "User creation failed",
                };

                var res = await SendMailAsync(createUserRequest.Email, "Welcome Message",
                    $"Hi {createUserRequest.UserName}, \nThank you for your successful registration on our platform.\nWe're here to serve you better.\nRegards");

                var createdUserResponse = mapper.Map<CreatedUserResponse>(user);
                return new APIResponse<CreatedUserResponse>
                {
                    IsSuccessful = true,
                    Message = "User created successfully",
                    Data = createdUserResponse
                };
            }
            catch(Exception exc)
            {
                response.Message = $"User registration successful but an exception occured during WelCome Message: {exc.Message}"; 
            }

            response.IsSuccessful = false;
            return response;

        }

        public async Task<APIResponse<UserResponse>> GetUserByEmail(string email)
        {
          if(string.IsNullOrEmpty(email)) return new APIResponse<UserResponse> { IsSuccessful = false, Message = "Email cannot be empty"};

          var user = await userRepository.GetUserByEmailOrUserName(email);
            if (user is null) return new APIResponse<UserResponse> { IsSuccessful = false, Message = "No user found with the provided email" };
            var userResponse = mapper.Map<UserResponse>(user);
            return new APIResponse<UserResponse> { IsSuccessful = true, Data = userResponse, Message = "User fetched"};
        }

        public async Task<APIResponse<UserResponse>> Login(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return new APIResponse<UserResponse> { IsSuccessful = false, Message = "Kindly provide a valid email" };
            if (string.IsNullOrEmpty(request.Password))
                return new APIResponse<UserResponse> { IsSuccessful = false, Message = "Kindly provide a valid password" };
            var user = await userRepository.GetUserByEmailOrUserName(request.Email);
            if (user is null)
                return new APIResponse<UserResponse> { IsSuccessful = false, Message = "No user found with the provided details" };
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PaswordHash))
                return new APIResponse<UserResponse> { IsSuccessful = false, Message = "Invalid credentials" };
            var token = await jWTService.GenerateJwtToken(user);

            if (string.IsNullOrEmpty(token))
                return new APIResponse<UserResponse> { IsSuccessful = false, Message = "Failed to generate login token" };
            var userResponse = new UserResponse
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
            return new APIResponse<UserResponse> { Message = "Successfully logged in", IsSuccessful = true, Data = userResponse };
            
        }


        private async Task<bool> SendMailAsync(string recipientmail, string subject, string body)
        {
            var emailMessage = new EmailMessage
            {
                To = recipientmail,
                Subject = subject,
                Body = body
            };
            await eMailService.SendEmailAsync(emailMessage);
            return true;
        }
        private static APIResponse<CreatedUserResponse> UserDataValidation(CreateUserRequest createUserRequest)
        {
            if (string.IsNullOrEmpty(createUserRequest.Email))
                return new APIResponse<CreatedUserResponse>
                {
                    IsSuccessful = false,
                    Message = "Kindly provide a valid email"
                };
            if (string.IsNullOrEmpty(createUserRequest.UserName))
                return new APIResponse<CreatedUserResponse>
                {
                    IsSuccessful = false,
                    Message = "Kindly provide a valid user name"
                };

            if (string.IsNullOrEmpty(createUserRequest.Password))
                return new APIResponse<CreatedUserResponse>
                {
                    IsSuccessful = false,
                    Message = "Kindly provide a valid Password"
                };
            return new APIResponse<CreatedUserResponse> { IsSuccessful = true };
        }
    }
}
