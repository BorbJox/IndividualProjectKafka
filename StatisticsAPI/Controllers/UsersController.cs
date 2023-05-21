using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatisticsAPI.Models;
using StatisticsAPI.Services;

namespace StatisticsAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        readonly IUserService _userService;
        readonly IJwtProvider _jwtProvider;

        public UsersController(IUserService userService, IJwtProvider jwtProvider)
        {
            _userService = userService;
            _jwtProvider = jwtProvider;
        }

        [HttpGet]
        public IEnumerable<User> Index()
        {
            return _userService.GetUsers();
        }

        [HttpGet]
        public string GetJwtToken(string name) 
        {
            User? user = _userService.GetByName(name);

            if (user == null)
            {
                return "User not found.";
            }

            string token = _jwtProvider.Generate(user);

            return token;
        }

        [HttpGet]
        public User? GetUser(string name)
        {
            return _userService.GetByName(name);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public User Create(User user)
        {
            return _userService.Write(user);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public User Update(User user)
        {
            return _userService.Update(user);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public ActionResult Delete(User user)
        {
            _userService.Delete(user);
            return NoContent();
        }
    }
}
