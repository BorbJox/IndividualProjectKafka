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

        //// GET: APIKeysController/Details/5
        //public ActionResult Details(int id)
        //{
        //}

        //// GET: APIKeysController/Create
        //public ActionResult Create()
        //{
        //}

        //// GET: APIKeysController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //}

        //// GET: APIKeysController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //}
    }
}
