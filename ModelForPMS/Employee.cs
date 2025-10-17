using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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



        [JsonIgnore]
        public ICollection<EmployeeRole>? EmployeeRoles { get; set; }

        [JsonIgnore]
        public ICollection<ProjectAssignment>? ProjectAssignments { get; set; }

        [JsonIgnore]
        public ICollection<Leave>? Leaves { get; set; }

    }
}
