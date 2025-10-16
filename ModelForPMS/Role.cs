using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelForPMS
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public ICollection<EmployeeRole> EmployeeRoles { get; set; }
    }
}
