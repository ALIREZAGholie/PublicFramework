using System.Security.Claims;

namespace Webgostar.Framework.Base.IBaseServices
{
    public interface IAuthService
    {
        long GetUserId();
        string GetUserFullName();
        Guid? GetUserGuid();
        string GetUserToken();
        long GetRoleId();
        Guid GetRoleGuid();
        List<Guid> GetRoles();
        List<long> GetRolesLong();
        string GetRoleExpDate();
        long GetExpDate();
        long GetExpires();
        List<Claim> GetClaims();
        string GetIpAddress();
        void CheckRoleValidation(long RoleID);
        void CheckRoleValidationByRoleGuid(Guid RoleGUID);
    }
}
