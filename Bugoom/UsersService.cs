using Microsoft.EntityFrameworkCore;
using Bugoom.Interfaces;

namespace Bugoom
{
    public class UsersService : IUsersService
    {
        BuggingContext context;
        public UsersService(BuggingContext context)
        { 
            this.context = context;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var users = await context.Users.ToListAsync();
            return users;
        }
    }
}