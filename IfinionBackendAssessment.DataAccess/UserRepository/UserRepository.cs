using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace IfinionBackendAssessment.DataAccess.UserRepository
{
    public class UserRepository: GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context): base(context) 
        {
          _context = context;
        }
        public async Task<User> GetUserByEmailOrUserName(string emailOrUserName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == emailOrUserName || x.UserName == emailOrUserName);
            return user!;
        }

        public async Task<User> GetUserById(int Id)
        {
            return  await _context.Users.FindAsync(Id);
        }
    }
}
