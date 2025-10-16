using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelForPMS
{
    public class EmployeeRole
    {

            public int EmployeeId { get; set; }
            public Employee Employee { get; set; }
            public int RoleId { get; set; }
            public Role Role { get; set; }

    }
}
