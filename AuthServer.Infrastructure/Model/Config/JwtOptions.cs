using System;
using System.Collections.Generic;
using System.Text;

namespace AuthServer.Infrastructure.Model.Config
{
    public class JwtOptions
    {
        public int ExpiresInMinutes { get; set; }
        public string Secret { get; set; }
        public string issuer { get; set; }
    }
}
