﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AuthServer.Domain.Interfaces
{
    public  interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
