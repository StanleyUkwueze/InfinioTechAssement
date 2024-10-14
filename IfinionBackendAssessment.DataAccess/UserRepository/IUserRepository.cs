using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.DataAccess.UserRepository
{
    public interface IUserRepository: IGenericRepository<User>
    {
        Task<User> GetUserByEmailOrUserName(string email);
        Task<User> GetUserById(int Id);

    }
}
