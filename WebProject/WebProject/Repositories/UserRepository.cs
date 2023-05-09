using WebProject.Data;
using WebProject.Models;

namespace WebProject.Repositories
{
    public class UserRepository : Repository<user> , IUserRepository
    {
        private AppDbContext _context;
        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(user users)
        {
            var objFromDb = _context.users.FirstOrDefault(s => s.Id == users.Id);
            if (objFromDb != null)
            {
                objFromDb.last_name = users.last_name;
                objFromDb.first_name = users.first_name;
                objFromDb.Email = users.Email;
                objFromDb.PasswordHash = users.PasswordHash;
                objFromDb.phone_number = users.phone_number;
                objFromDb.Address = users.Address;
            }
        }
    }   
}
