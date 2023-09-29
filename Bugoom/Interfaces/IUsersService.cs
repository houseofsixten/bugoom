namespace Bugoom.Interfaces
{
    public interface IUsersService
    {
        void Add(User user);
        Task<IEnumerable<User>> GetAll();
    }
}
