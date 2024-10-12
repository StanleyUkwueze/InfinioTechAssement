using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IfinionBackendAssessment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserService userService) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUserRequest request)
        {
            var response = await userService.CreateUser(request);

            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await userService.Login(request);

            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }
    }
}
