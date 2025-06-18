namespace Webgostar.Framework.Infrastructure.InfrastructureExtentisions
{
    public enum ErrorCode
    {
        Internal = 0,
        VersionConflict = 1, // NuGet package verions different
        NotFound = 2,
        BadRequest = 3,
        Conflict = 4,
        Other = 5,
        Unauthorized = 6,
        ItemAlreadyExists = 7,
        UnprocessableEntity = 8
    }
}
