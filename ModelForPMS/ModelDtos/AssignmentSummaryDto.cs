using System;
using System.Text.Json.Serialization;

// DTO: Contains only the specific fields requested from all tables, 
// ensuring lightweight data transfer.
public class AssignmentSummaryDto
{
    // From Employee
    public string EmployeeFirstName { get; set; }

    // From Role
    public string RoleName { get; set; }

    // From Project
    public string ProjectName { get; set; }

    // From ProjectAssignment
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Optional: Include AssignmentId for tracking/editing purposes
    public int AssignmentId { get; set; }
}
