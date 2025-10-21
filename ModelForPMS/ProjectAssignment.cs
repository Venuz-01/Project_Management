using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModelForPMS
{
    public class ProjectAssignment
    {
        [Key]
        public int AssignmentId { get; set; }
        public int ProjectId { get; set; }

        [JsonIgnore]
        public Project? Project { get; set; }

        public int EmployeeId { get; set; }


        [JsonIgnore]
        public Employee? Employee { get; set; }

        public int RoleId { get; set; }

        public Role? Role { get; set; }
        public decimal AllocationPercent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Billable { get; set; } = true;


        public int GetWorkedDays()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                // Calculate total days between start and end date
                int totalDays = (EndDate.Value - StartDate.Value).Days + 1;
                // Calculate worked days based on allocation percentage
                return (int)(totalDays * (AllocationPercent / 100));
            }
            return 0;
        }
    }
}
