using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModelForPMS
{
    public class Leave
    {
        [Key]
        public int LeaveId { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        [JsonIgnore]
        public Employee? Employee { get; set; }

        public int? ProjectId { get; set; }

        
        [JsonIgnore]
        public Project? Project { get; set; }

        [MaxLength(20)]
        public string LeaveType { get; set; }

        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }

        [MaxLength(255)]
        public string? Reason { get; set; }

    }
}

