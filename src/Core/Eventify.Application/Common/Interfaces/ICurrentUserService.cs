namespace Eventify.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
        IEnumerable<string> GetRoles();
    }
}
