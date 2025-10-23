using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelForPMS
{
    public class Holiday
    {
        [Key]
        public int HolidayId { get; set; }
        public DateOnly Date { get; set; }
        public string Description { get; set; }

        public int? AssignmentId { get; set; }

        public ICollection<ProjectAssignment>? ProjectAssignments { get; set; }
    }
}
