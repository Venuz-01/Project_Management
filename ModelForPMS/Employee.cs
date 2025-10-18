using ModelForPMS;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EmployeeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }


    public string Email { get; set; }

    public ICollection<Role>? Roles { get; set; }

    [JsonIgnore]
    public ICollection<ProjectAssignment>? ProjectAssignments { get; set; }

    [JsonIgnore]
    public ICollection<Leave>? Leaves { get; set; }

}