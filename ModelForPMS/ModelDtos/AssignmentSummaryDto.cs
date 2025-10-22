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
    public DateTime? EmployeeStartDate { get; set; }
    public DateTime? EmployeeEndDate { get; set; }

    public int WorkedDays;

    // Optional: Include AssignmentId for tracking/editing purposes
    public string ClientName { get; set; }

    public string ClientEmail { get; set; }


    public decimal? RatePerDay { get; set; }

    public DateTime? ProjectStartDate;

    public DateTime? ProjectEndDate;

    public int Budget;
}
