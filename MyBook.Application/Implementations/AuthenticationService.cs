using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyBook.Application.Interfaces;
using MyBook.Domain.Models;
using MyBook.Domain;
using MyBook.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace MyBook.Application.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailService _emailServices;
        private readonly EmailSettings _emailSettings;
       // private readonly ILogger _logger;
        //private readonly JwtTokenGeneratorService _tokenGenerator;
        private readonly IAuthenticationRepository _authentication;

        public AuthenticationService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<EmailSettings> emailSettings,  IAuthenticationRepository authentication)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_emailServices = new MailService(emailSettings);
            _emailSettings = emailSettings.Value;
           // _logger = logger;
           // _tokenGenerator = tokenGenerator;
            _authentication = authentication;
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

                    var resetPasswordUrl = "https://localhost:7075/confirm-email?email=" + Uri.EscapeDataString(user.Email) + "&token=" + Uri.EscapeDataString(emailConfirmationToken);

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

       /* public async Task<ApiResponse<string>> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found.", 404, null, new List<string>());
                }

                var result = await _signInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var token = _tokenGenerator.GenerateJwtToken(user);
                    return new ApiResponse<string>(true, "Login successful.", 200, token, new List<string>());
                }
                else if (result.IsLockedOut)
                {
                    return new ApiResponse<string>(false, "Account is locked out. Please try again later.", 403, null, new List<string>());
                }
                else
                {
                    return new ApiResponse<string>(false, "Login failed. Invalid email or password.", 401, null, new List<string>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                var errorList = new List<string> { ex.Message };
                return ApiResponse<string>.Failed(false, "Error occurred during login", 500, errorList);
            }
        }*/
    }
}
