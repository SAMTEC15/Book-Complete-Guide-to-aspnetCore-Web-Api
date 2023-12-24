using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBook.Application.Interfaces;
using MyBook.Domain;
using MyBook.Domain.Dto;
using MyBook.Domain.Models;
using System.Net;

namespace MyBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
                // Set other user properties here
            };

            var response = await _authenticationService.RegisterAsync(user, model.Password);

            if (response.IsSuccess)
            {
                var reply = new APIResponse<string>
                {
                    StatusCode = HttpStatusCode.Created,
                    IsSuccess = true,
                    ErrorMessages = new List<string>(),
                    Result = "User Registration Successfully"
                };
                return Ok(reply);
            }
            var responses = new APIResponse<string>
            {
                StatusCode = HttpStatusCode.BadRequest,
                IsSuccess = false,
                ErrorMessages = new List<string>(),
                Result = "User Registration Failed"
            };

            return BadRequest(responses);
        }

        [HttpPost("login")]
        public async Task<ActionResult<APIResponse<string>>> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                var responses = new APIResponse<string>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = new List<string>(),
                    Result = "invalid input."
                    // Example result of type string
                };
                return responses;                
            }

            var response = await _authenticationService.LoginAsync(model.Email, model.Password);

            if (response.IsSuccess)
            {
                var responses = new APIResponse<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    ErrorMessages = new List<string>(),
                    Result =  "Login Success.",
                    Data = response.Data
                    // Example result of type string
                };
                return Ok(responses);
            }

            return BadRequest();
        }


    }
}
