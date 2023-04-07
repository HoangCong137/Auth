using System;
using System.Collections.Generic;
using System.Text;

namespace AuthServer.Infrastructure.Model
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
