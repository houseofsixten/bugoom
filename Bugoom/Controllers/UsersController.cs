using Bugoom.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bugoom.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersService usersService;

        public UsersController(ILogger<UsersController> logger, IUsersService usersService)
        {
            _logger = logger;
            this.usersService = usersService;
        }

        [HttpGet(Name = "GetAllUsers")]
        public async Task<IEnumerable<User>> GetAll()
        {
            var allUsers = await usersService.GetAll();
            return allUsers;
        }
    }
}