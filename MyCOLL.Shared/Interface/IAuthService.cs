namespace MyCOLL.Shared.Interface;

public interface IAuthService
{
    Task Login(string token);
    Task Logout();
}