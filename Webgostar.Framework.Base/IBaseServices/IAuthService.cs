using System.Security.Claims;

namespace Webgostar.Framework.Base.IBaseServices
{
    public interface IAuthService
    {
        string GetUserId();
        string GetUserFullName();
        string GetUserToken();
        string GetRoleId();
        List<string> GetRoles();
        string GetRoleExpDate();
        DateTime? GetExpDate();
        DateTime? GetExpires();
        List<Claim> GetClaims();
        string GetIpAddress();
        void CheckRoleValidation(string RoleID);
        void CheckRoleValidationByRoleGuid(string RoleGUID);
    }
}
