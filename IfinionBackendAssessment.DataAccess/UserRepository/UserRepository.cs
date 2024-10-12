using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
