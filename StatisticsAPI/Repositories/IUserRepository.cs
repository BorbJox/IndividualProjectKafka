using StatisticsAPI.Models;

namespace StatisticsAPI.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User?> GetUser(int userId);
        Task<User?> GetUserByName(string name);
        Task<User> AddUser(User user);
        Task<User?> UpdateUser(User user);
        void DeleteUser(int userId);
    }
}
