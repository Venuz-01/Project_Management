using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelForPMS
{
    public class ProjectAssignment
    {
        [Key]
        public int AssignmentId { get; set; }
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public decimal AllocationPercent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Billable { get; set; } = true;
    }
}
