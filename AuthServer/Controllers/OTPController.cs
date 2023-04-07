using AuthServer.Infrastructure.Service.OTP;
using AuthServer.Infrastructure.ServiceModel;
using AuthServer.Infrastructure.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Controllers
{
    [Route("api/otp")]
    [ApiController]
    public class OTPController : Controller
    {
        private readonly IOTPService _otpService;

        public OTPController(IOTPService AuthService)
        {
            _otpService = AuthService;
        }

        [HttpPost("send-otp")]
        public async Task<ServiceResponse> post([FromBody] OtpCodeViewModel model) => await _otpService.SendOtp(model.Email,"vi");
    }
}
