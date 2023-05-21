using StatisticsAPI.Models;
using StatisticsAPI.Repositories;

namespace StatisticsAPI.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        User? GetByName(string name);
        User Write(User user);
        User Update(User user);
        void Delete(User user);
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

        public User Write(User user)
        {
            var existingUser = _repo.GetUserByName(user.Name).Result;
            if (existingUser == null)
            {
                _ = _repo.AddUser(user).Result;
            }
            return _repo.GetUserByName(user.Name).Result ?? throw new Exception("Something went very wrong when writing user.");
        }

        public User Update(User user)
        {
            User? updated = _repo.UpdateUser(user).Result;
            return updated ?? throw new Exception("User not found.");
        }

        public void Delete(User user)
        {
            _repo.DeleteUser(user.Id);
        }
    }
}
