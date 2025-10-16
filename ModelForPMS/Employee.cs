using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelForPMS
{
   public class Employee
    {
        [Key]

        public int EmployeeId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public decimal? DefaultDailyRate { get; set; }
            public ICollection<EmployeeRole> EmployeeRoles { get; set; }
            public ICollection<ProjectAssignment> ProjectAssignments { get; set; }
            public ICollection<Leave> Leaves { get; set; }

    }
}
