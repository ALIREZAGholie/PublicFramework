using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Webgostar.Framework.Base.IBaseServices;
using Webgostar.Framework.Infrastructure.InfrastructureExceptions;

namespace Webgostar.Framework.Infrastructure.BaseServices
{
    public class AuthService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        : IAuthService
    {
        public string GetUserId()
        {
            try
            {
                var userId = httpContextAccessor.HttpContext?.User?.Claims.First(a => a.Type == "Id").Value ?? "0";
                return userId;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetUserFullName()
        {
            try
            {
                if (!getClaims().Any())
                {
                    return "کاربر مهمان";
                }
                var fullName = getClaims().FirstOrDefault(a => a.Type == "FullName")!.Value ?? "کاربر مهمان";

                return fullName;
            }
            catch (Exception)
            {
                return "کاربر مهمان";
            }
        }

        public Guid? GetUserGuid()
        {
            try
            {
                var userGuid = getClaims().FirstOrDefault(a => a.Type == "UserGuid")!.Value ?? null;

                return Guid.Parse(userGuid);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetUserToken()
        {
            try
            {
                if (httpContextAccessor.HttpContext != null)
                {
                    string token = httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];

                    token = token != null ? token.Replace("Bearer ", "") : "";

                    return token;
                }

                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetRoleId()
        {
            try
            {
                var roleId = httpContextAccessor.HttpContext.Request.Headers["RoleId"].First() ?? "0";

                CheckRoleValidation(roleId);

                return roleId;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public List<string> GetRoles()
        {
            try
            {
                var Roles = getClaims()
                    .Where(a => a.Type == "Role")
                    .Select(claim => claim.Value)
                    .ToList();

                return Roles;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public List<string> GetRolesLong()
        {
            try
            {
                var rolesStr = httpContextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(a => a.Type == "RolesId").Value;

                var roles = JsonConvert.DeserializeObject<List<RoleAuthService>>(rolesStr);

                return roles.Select(x => x.RoleId).ToList();
            }
            catch (Exception)
            {
                return [];
            }
        }

        public string GetRoleExpDate()
        {
            var roleExpDate = getClaims().FirstOrDefault(a => a.Type == "RoleExpireDate")!.Value;

            return roleExpDate;
        }

        public DateTime? GetExpDate()
        {
            try
            {
                var expires = getClaims().FirstOrDefault(a => a.Type == "ExpireDate")!.Value;

                return DateTime.Parse(expires);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DateTime? GetExpires()
        {
            try
            {
                var expires = GetClaims().FirstOrDefault(a => a.Type == "Expires")!.Value;

                return DateTime.Parse(expires);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Claim> GetClaims()
        {
            try
            {
                var claims = getClaims().ToList();

                return claims;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private IEnumerable<Claim> getClaims()
        {
            var accessToken = GetUserToken();

            if (string.IsNullOrEmpty(accessToken)) return null;

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration["JwtConfig:SignInKey"] ?? string.Empty));
            JwtSecurityTokenHandler tokenHandler = new();

            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtConfig:Issuer"],
                ValidAudience = configuration["JwtConfig:Audience"],
                IssuerSigningKey = securityKey
            };

            try
            {
                tokenHandler.ValidateToken(accessToken, validationParameters, out _);

                if (tokenHandler.ReadToken(accessToken) is JwtSecurityToken jsonToken)
                {
                    return jsonToken.Claims.ToList();
                }
            }
            catch (Exception)
            {
                return new List<Claim>();
            }

            return null;
        }

        public string GetIpAddress()
        {
            //var ip = _httpContextAccessor.HttpContext.Request.Headers["UserIpAddress"].ToString();
            var ip = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            return ip;
        }

        public void CheckRoleValidation(string roleId)
        {
            try
            {
                var userRoles = GetRolesLong();

                var exist = userRoles.Any(r => r == roleId);

                if (!exist)
                {
                    throw new AuthException("نقش کاربر معتبر نیست");
                }
            }
            catch (AuthException e)
            {
                throw e;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CheckRoleValidationByRoleGuid(string roleId)
        {
            try
            {
                var roleExpDateJson = GetRoleExpDate();

                var jArray = JArray.Parse(roleExpDateJson);

                var roleExpDate =
                    jArray.FirstOrDefault(token => token["Role"].ToString() == roleId.ToString());

                var ExpDate = long.Parse(roleExpDate["ExpireDate"].ToString());
                var NowDate = DateTime.Now.Ticks;

                if (ExpDate > 0 && NowDate > ExpDate)
                {
                    throw new AuthException("نقش کاربر معتبر نیست");
                }

                var UserRoles = GetRoles();
                var exist = UserRoles.Any(r => r == roleId);
                if (!exist)
                {
                    throw new AuthException("نقش کاربر معتبر نیست");
                }
            }
            catch (AuthException e)
            {
                throw e;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    internal record RoleAuthService
    {
        public string RoleId { get; set; }
    }
}
