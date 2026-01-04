using MyCOLL.Data.Data;

namespace MyCOLL.Interface;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetAllUsers();
    Task<ApplicationUser?> GetUserById(string id);
    Task AddUser(ApplicationUser user);
    Task UpdateUser(ApplicationUser user);
    Task DeleteUser(string id);
}