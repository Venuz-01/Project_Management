using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelForPMS;

namespace RepositoriesForPMS.Interfaces
{
    public interface IUserRepository
    {
        User ValidateUser(string email, string password, string role);
    }

}
