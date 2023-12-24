using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MyBook.Application.Interfaces;
using MyBook.Domain;
using MyBook.Domain.Models;
using MyBook.Persistence.Repositories.Interfaces;
using System.Net;

namespace MyBook.Application.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailService _emailServices;
        private readonly EmailSettings _emailSettings;
        //private readonly IAuthenticationRepository _authentication;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthenticationService(UserManager<ApplicationUser> userManager, ITokenService tokenService, RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager, IOptions<EmailSettings> emailSettings)
        {
            
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailServices = new EmailService(emailSettings);
            _emailSettings = emailSettings.Value;
           // _authentication = authentication;
           _roleManager = roleManager;
        }

        public async Task<APIResponse<string>> RegisterAsync(ApplicationUser user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);


                if (result.Succeeded)
                {
                    // Generate email confirmation token
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var resetPasswordUrl = "https://localhost:7141/confirm-email?email=" + Uri.EscapeDataString(user.Email) + "&token=" + Uri.EscapeDataString(emailConfirmationToken);

                    // Send email confirmation
                    var mailRequest = new MailRequest
                    {
                        ToEmail = user.Email,
                        Subject = "Confirm Your Email",
                        // You can customize the email body as needed
                        Body = $"Thank you for registering! Please confirm your email address by clicking the link below:<br>" +
                               $"<a href='{resetPasswordUrl}'>Confirm Email</a>"
                    };

                    await _emailServices.SendEmailConfirmationAsync(mailRequest, emailConfirmationToken);

                    // Update the user to mark as unconfirmed (optional, based on your logic)
                    user.EmailConfirmed = false;
                    await _userManager.UpdateAsync(user);
                    var response = new APIResponse<string>
                    {
                        StatusCode = HttpStatusCode.Created,
                        IsSuccess = true,
                        ErrorMessages = new List<string>(),
                        Result = $"{emailConfirmationToken}"
                        // Example result of type string
                    };

                    return response;
                }
                else
                {
                    var response = new APIResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotAcceptable,
                        IsSuccess = false,
                        ErrorMessages = new List<string>(),
                        Result = "Registration failed"
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error occurred during registration");
                var response = new APIResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string>(),
                    Result = "Error occurred during registration"
                    // Example result of type string
                };
                return response;
            }
        }
      
        public async Task<APIResponse<string>> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    var response = new APIResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        ErrorMessages = new List<string>(),
                        Result = "User not found"
                        // Example result of type string
                    };
                    return response;
                }

                var result = await _signInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var token = _tokenService.GenerateJwtToken(user);
                    var response = new APIResponse<string>
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSuccess = true,
                        ErrorMessages = new List<string>(),
                        Result = "Login Successful",
                        Data = token
                        // Example result of type string
                    };
                    return response;
                }
                else if (result.IsLockedOut)
                {
                    var response = new APIResponse<string>
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        IsSuccess = false,
                        ErrorMessages = new List<string>(),
                        Result = "Account is locked out. Please try again later."
                        // Example result of type string
                    };
                    return response;

                }
                else
                {
                    var response = new APIResponse<string>
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string>(),
                        Result = "Login failed. Invalid email or password."
                        // Example result of type string
                    };
                    return response;

                }
            }
            catch (Exception ex)
            {

                var response = new APIResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string>(),
                    Result = "Error occurred during login"
                    // Example result of type string
                };
                return response;

            }
        }



    }
}
