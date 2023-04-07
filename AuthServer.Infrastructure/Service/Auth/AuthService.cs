
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuthServer.Infrastructure.Common.Service;
using AuthServer.Domain.Entities;
using AuthServer.Domain.Shared;
using AuthServer.Infrastructure.Model;
using AuthServer.Infrastructure.Model.Auth;
using AuthServer.Infrastructure.Model.Config;
using AuthServer.Domain.Interfaces;
using AuthServer.Infrastructure.ServiceModel;
using AuthServer.Infrastructure.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace AuthServer.Infrastructure.Service.Auth
{
    public class AuthService : SResponse, IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtOptions _options;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IAsyncRepository<User> _repository;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<JwtOptions> options
                           ,IAsyncRepository<User> repository,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _options = options.Value;
            _jwtSecurityTokenHandler ??= new JwtSecurityTokenHandler();
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<ServiceResponse> CreateUserAsyncEmail(RegisterViewModelEmail User)
        {
            var connectionString = this.GetConnection();
            using var con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                var userRegister = new User()
                {
                    UserName = User.Email,
                    Email = User.Email,
                    FullName = User.Email,
                    Source = Settings.SOURCE
                };
                var userResult = await _userManager.FindByEmailAsync(User.Email);
                userRegister.Id = Guid.NewGuid();
                var xacThucId = await con.ExecuteScalarAsync<Guid>($"SELECT TOP 1 Id FROM OtpCodeLog WHERE Email = '{User.Email}' AND Code = '{User.OtpCode}' AND ThoiGianXacThuc IS NULL");
                if (!Guid.Empty.Equals(xacThucId))
                {
                    await _userManager.CreateAsync(userRegister, User.Password);
                    await _userManager.AddToRoleAsync(userRegister, Settings.ENDUSERROLE);
                    var InfoUser = _repository.ListAll().Where(x => x.Email == User.Email).FirstOrDefault();
                    await con.ExecuteAsync($"UPDATE OtpCodeLog SET ThoiGianXacThuc = GETDATE() WHERE Email = '{User.Email}' AND Code = '{User.OtpCode}'");
                    var result = new AuthenticateResponse
                    {
                        AccessToken = GenerateAccessToken(InfoUser.Id, InfoUser.UserName),
                        RefreshToken = generateRefreshToken(InfoUser.UserName).Token,
                        //UserInformation = InfoUser,
                    };
                    return Ok(result);
                }
                return Unauthorized(StatusCodes.Status401Unauthorized.ToString(), "Mã xác thực không đúng,vui lòng thử lại");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            
        }

        public async Task<ServiceResponse> CreateUserAsyncMobile(RegisterViewModelMobile User)
        {
            var userRegister = new User()
            {
                PhoneNumber = User.Mobile,
                UserName = User.Mobile,
                FullName = User.FullName
            };

            //var roles = new List<string>();
            //roles.Add("EndUser");
            userRegister.Id = Guid.NewGuid();
            var resultCreated = await _userManager.CreateAsync(userRegister, User.Password);

            var InfoUser = _repository.ListAll().Where(x => x.PhoneNumber == User.Mobile).FirstOrDefault();

            if (resultCreated.Succeeded)
            {
                var result = new AuthenticateResponse
                {
                    AccessToken = GenerateAccessToken(InfoUser.Id, InfoUser.UserName),
                    RefreshToken = generateRefreshToken(InfoUser.UserName).Token,
                    //UserInformation = InfoUser,
                };
                return Ok(result);
            }

            return Ok(resultCreated);
        }

        public async Task<ServiceResponse> AuthenticateEmailAsync(LoginViewEmailModel request)
        {
            var user = await _signInManager.PasswordSignInAsync(request.Email, request.Password,false,false);
            if (user.Succeeded)
            {
                var InfoUser = await _repository.FistOrDefaultAsync(x => x.Email == request.Email);
                var result = new AuthenticateResponse
                {
                    AccessToken = GenerateAccessToken(InfoUser.Id, InfoUser.UserName),
                    RefreshToken = generateRefreshToken(InfoUser.UserName).Token,
                    //UserInformation = InfoUser,
                };
                return Ok(result);
            }
            return Unauthorized(StatusCodes.Status401Unauthorized.ToString(), "Tài khoản không tồn tại");
        }

        public async Task<ServiceResponse> AuthenticateMobileAsync(LoginViewMobileModel request)
        {
            var user = await _signInManager.PasswordSignInAsync(request.Mobile, request.Password, false, false);
            if (user.Succeeded)
            {
                var InfoUser = await _repository.FistOrDefaultAsync(x => x.PhoneNumber == request.Mobile);
                var result = new AuthenticateResponse
                {
                    AccessToken = GenerateAccessToken(InfoUser.Id, InfoUser.UserName),
                    RefreshToken = generateRefreshToken(InfoUser.UserName).Token,
                    //UserInformation = InfoUser,
                };
                return Ok(result);
            }
            return Unauthorized();
        }

        public async Task<ServiceResponse> UpdatePassword(UpdatePasswordViewModel model)
        {
            var user = _repository.FistOrDefault(x => x.PhoneNumber == model.Mobile && model.Mobile != null || x.Email == model.Email && model.Email != null );

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token , model.NewPassword);

            return Ok(result);
        }

        private string GenerateAccessToken(Guid userId, string username/*, object permission, object canBoInfo*/)
        {
            var claims = new[] {
                new Claim(Claims.UserId, userId.ToString(), ClaimValueTypes.String),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                //new Claim(Claims.Permissions, Newtonsoft.Json.JsonConvert.SerializeObject(permission)),
                //new Claim(Claims.CanBoViewModel, Newtonsoft.Json.JsonConvert.SerializeObject(canBoInfo))
            };
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)), SecurityAlgorithms.HmacSha256Signature);
       
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_options.ExpiresInMinutes),
                SigningCredentials = signingCredentials,
                Issuer = _options.issuer,
                Audience = _options.issuer,
            };
            return _jwtSecurityTokenHandler.WriteToken(_jwtSecurityTokenHandler.CreateToken(tokenDescriptor));    

        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

        public async Task<ServiceResponse> AuthenticateFacebookEmailAsync(LoginViewFacebookEmailModel request)
        {


            var user = await _repository.FistOrDefaultAsync(q => q.FacebookId.Equals(request.FacebookId) || q.Email.Equals(request.Email));
            user.FacebookId = request.FacebookId;
            if (user != null)
            {
                await _repository.UpdateAsync(user);
                var result = new AuthenticateResponse
                {
                    AccessToken = GenerateAccessToken(user.Id, user.UserName),
                    RefreshToken = generateRefreshToken(user.UserName).Token,
                    //UserInformation = InfoUser,
                };
                return Ok(result);
            }
            return Unauthorized(StatusCodes.Status401Unauthorized.ToString(),"Tài khoản không tồn tại");
        }

        public async Task<ServiceResponse> AuthenticateAppleEmailAsync(LoginViewAppleEmailModel request)
        {
            var user = await _repository.FistOrDefaultAsync(q => q.AppleId.Equals(request.AppleId) || q.Email.Equals(request.Email));
            user.AppleId = request.AppleId;
            if (user != null)
            {
                await _repository.UpdateAsync(user);
                var result = new AuthenticateResponse
                {
                    AccessToken = GenerateAccessToken(user.Id, user.UserName),
                    RefreshToken = generateRefreshToken(user.UserName).Token,
                    //UserInformation = InfoUser,
                };
                return Ok(result);
            }
            return Unauthorized(StatusCodes.Status401Unauthorized.ToString(), "Tài khoản không tồn tại");
        }

        public string GetConnection()
        {
            var connection = _configuration.GetSection("ConnectionStrings").GetSection("winwinkorea").Value;
            return connection;
        }

        public async Task<ServiceResponse> UpdateForgetPassword(UpdateForgetPasswordViewModel model)
        {
            var connectionString = this.GetConnection();
            using var con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                var xacThucId = await con.ExecuteScalarAsync<Guid>($"SELECT TOP 1 Id FROM OtpCodeLog WHERE Email = '{model.Email}' AND Code = '{model.OtpCode}' AND ThoiGianXacThuc IS NULL");
                if (!Guid.Empty.Equals(xacThucId))
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var passwordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
                    user.PasswordHash = passwordHash;
                    await _userManager.UpdateAsync(user);
                    await con.ExecuteAsync($"UPDATE OtpCodeLog SET ThoiGianXacThuc = GETDATE() WHERE Email = '{model.Email}' AND Code = '{model.OtpCode}'");
                    var result = new AuthenticateResponse
                    {
                        AccessToken = GenerateAccessToken(user.Id, user.UserName),
                        RefreshToken = generateRefreshToken(user.UserName).Token,
                        //UserInformation = InfoUser,
                    };
                    return Ok(result);
                }
                return Unauthorized();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }
    }
}
