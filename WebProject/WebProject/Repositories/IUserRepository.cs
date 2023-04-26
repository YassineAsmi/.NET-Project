using WebProject.Models;

namespace WebProject.Repositories
{
    public interface IUserRepository : IRepository<user>
    {
        void Update(user users);
    }
}
