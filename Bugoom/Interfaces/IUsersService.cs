namespace Bugoom.Interfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> GetAll();
    }
}
