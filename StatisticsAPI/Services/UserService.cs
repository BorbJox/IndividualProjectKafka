using StatisticsAPI.Models;
using StatisticsAPI.Repositories;

namespace StatisticsAPI.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        User? GetByName(string name);
    }

    public class UserService : IUserService
    {
        readonly IUserRepository _repo;

        public UserService(IUserRepository userRepository)
        {
            _repo = userRepository;
        }

        public IEnumerable<User> GetUsers()
        {
            return _repo.GetUsers().Result;
        }

        public User? GetByName(string name) 
        {
            return _repo.GetUserByName(name).Result;
        }
    }
}
