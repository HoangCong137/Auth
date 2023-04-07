using AuthServer.Infrastructure.ServiceModel;
using AuthServer.Infrastructure.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Service
{
    public interface IAuthService
    {
        Task<ServiceResponse> CreateUserAsyncEmail(RegisterViewModelEmail User);
        Task<ServiceResponse> CreateUserAsyncMobile(RegisterViewModelMobile User);
        Task<ServiceResponse> AuthenticateEmailAsync(LoginViewEmailModel request);
        Task<ServiceResponse> AuthenticateMobileAsync(LoginViewMobileModel request);
        Task<ServiceResponse> UpdatePassword(UpdatePasswordViewModel model);
        Task<ServiceResponse> AuthenticateFacebookEmailAsync(LoginViewFacebookEmailModel request);
        Task<ServiceResponse> AuthenticateAppleEmailAsync(LoginViewAppleEmailModel request);
        Task<ServiceResponse> UpdateForgetPassword(UpdateForgetPasswordViewModel model);
    }
}
