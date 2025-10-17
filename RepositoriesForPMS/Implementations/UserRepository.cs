using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;

namespace RepositoriesForPMS.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = new()
        {
            new User { UserId = 1, UserName = "Sales Lead", Email = "sales@pms.com", Password = "sales123", UserRole = "Sales Manager" },
            new User { UserId = 2, UserName = "Engineering Head", Email = "engineer@pms.com", Password = "eng123", UserRole = "Engineer Manager" },
            new User { UserId = 3, UserName = "HR Lead", Email = "hr@pms.com", Password = "hr123", UserRole = "HR Manager" },
            new User { UserId = 4, UserName = "Finance Lead", Email = "finance@pms.com", Password = "fin123", UserRole = "Finance Manager" }
        };

        public User ValidateUser(string email, string password, string role)
        {
            return _users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password &&
                u.UserRole.Equals(role, StringComparison.OrdinalIgnoreCase));
        }
    }

}
