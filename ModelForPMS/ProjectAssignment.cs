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
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool Billable { get; set; } = true;

        [JsonIgnore]
        public int LeaveId { get; set; }
        [JsonIgnore]
        public ICollection<Leave>? Leaves { get; set; }
        [JsonIgnore]
        public int HolidayId { get; set; }
        [JsonIgnore]
        public ICollection<Holiday>? Holidays { get; set; }
        public int GetWorkedDays()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                // Calculate total days between start and end date
                int totalDays = (EndDate.Value.DayNumber - StartDate.Value.DayNumber) ;
                // Calculate worked days based on allocation percentage
                return (int)(totalDays );
            }
            return 0;
        }
    }
}
