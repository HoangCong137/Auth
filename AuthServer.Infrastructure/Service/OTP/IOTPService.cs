using AuthServer.Infrastructure.ServiceModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Service.OTP
{
    public interface IOTPService
    {
        Task<ServiceResponse> SendOtp(String[] phones, String content, int type, String sender);
        Task<ServiceResponse> SendOtp(string email,string maLanguage);
    }
}
