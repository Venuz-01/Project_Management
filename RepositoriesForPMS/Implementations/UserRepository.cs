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
    new User { UserId = 1, UserName = "Venugopal Adicherla", Email = "venugopal.adicherla@abstract-group.com", Password = "abstract", UserRole = "Sales Manager" },
    new User { UserId = 2, UserName = "Saika Maqbool", Email = "saika.maqbool@abstract-group.com", Password = "abstract", UserRole = "Engineer Manager" },
    new User { UserId = 3, UserName = "Prasanth Poluparthi", Email = "prasanth.poluparthi@abstract-group.com", Password = "abstract", UserRole = "HR Manager" },
    new User { UserId = 4, UserName = "Sameer Ali Mohammed", Email = "sameer.alimohammed@abstract-group.com", Password = "abstract", UserRole = "Finance Manager" }
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
