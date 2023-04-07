using AuthServer.Infrastructure.ServiceModel;
using AuthServer.Infrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AuthServer.Infrastructure.Service;
using AuthServer.Domain.Entities;
using System;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthServer.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthService _authService;

        public AuthController(ILogger<HomeController> logger, UserManager<User> userManager, SignInManager<User> signInManager
                              , IAuthService AuthService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = AuthService;
        }
        
        [HttpPost("registerEmail")]
        public virtual async Task<ServiceResponse> Register(RegisterViewModelEmail model) => await _authService.CreateUserAsyncEmail(model);
        [Obsolete]
        [HttpPost("registerMobile")]
        public virtual async Task<ServiceResponse> RegisterMobile(RegisterViewModelMobile model) => await _authService.CreateUserAsyncMobile(model);
        [HttpPost("loginEmail")]
        public async Task<ServiceResponse> Login(LoginViewEmailModel model) => await _authService.AuthenticateEmailAsync(new LoginViewEmailModel
        {
            Password = model.Password,
            Email = model.Email
        });
        [Obsolete]
        [HttpPost("loginMobile")]
        public async Task<ServiceResponse> LoginMobile(LoginViewMobileModel model) => await _authService.AuthenticateMobileAsync(new LoginViewMobileModel
        {
            Password = model.Password,
            Mobile = model.Mobile
        });

        [HttpPost("updatePassword")]
        public virtual async Task<ServiceResponse> UpdateForgetPassword(UpdateForgetPasswordViewModel model) => await _authService.UpdateForgetPassword(model);
        [HttpPost("loginFacebook")]
        public async Task<ServiceResponse> LoginByFaceBook(LoginViewFacebookEmailModel model) => await _authService.AuthenticateFacebookEmailAsync(model);
        [HttpPost("loginApple")]
        public async Task<ServiceResponse> LoginByApple(LoginViewAppleEmailModel model) => await _authService.AuthenticateAppleEmailAsync(model);

    }
}
