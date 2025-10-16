using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelForPMS
{
    public class Leave
    {
        public int LeaveId { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int? ProjectId { get; set; }
        public Project? Project { get; set; }

        [MaxLength(20)]
        public string LeaveType { get; set; } = "FullDay"; 

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Applied"; 

        [MaxLength(255)]
        public string? Reason { get; set; }

    }
}

