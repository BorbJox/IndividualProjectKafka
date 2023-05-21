using Microsoft.EntityFrameworkCore;
using StatisticsAPI.Data;
using StatisticsAPI.Models;
using StatisticsAPI.Repositories;

namespace StatisticsAPI.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await appDbContext.Users.ToListAsync();
        }

        public async Task<User> GetUser(int userId)
        {
            return await appDbContext.Users.FirstAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByName(string name)
        {
            return await appDbContext.Users.FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task<User> AddUser(User user)
        {
            var result = await appDbContext.Users.AddAsync(user);
            await appDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<User?> UpdateUser(User user)
        {
            User? result = await appDbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            if (result != null)
            {
                result.Id = user.Id;
                result.Name = user.Name;
                result.APIKey = user.APIKey;
                result.IsAdmin = user.IsAdmin;

                await appDbContext.SaveChangesAsync();

                return result;
            }

            return null;
        }

        public async void DeleteUser(int userId)
        {
            var result = await GetUser(userId);
            if (result != null)
            {
                appDbContext.Users.Remove(result);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
