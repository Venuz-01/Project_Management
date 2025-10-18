using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelForPMS
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        // Existing fields (keep them unchanged)
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public DateTime Date { get; set; }
        public decimal WorkedDays { get; set; }
        public decimal RatePerDay { get; set; }

        // ✅ Newly added fields for FinanceController exports and invoice display

        // Client relationship
        public int ClientId { get; set; }
        public Client Client { get; set; }

        // Project period
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }

        // Read-only computed property for budget
        [NotMapped]
        public decimal Budget => WorkedDays * RatePerDay;

        // Optional display fields for easier mapping
        [NotMapped]
        public string EmployeeName => Employee != null ? $"{Employee.FirstName} {Employee.LastName}" : string.Empty;

        [NotMapped]
        public string ProjectName => Project != null ? Project.ProjectName : string.Empty;

        [NotMapped]
        public string ClientName => Client != null ? Client.ClientName : string.Empty;

        // Optional invoice metadata
        [MaxLength(100)]
        public string InvoiceNumber { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Generated"; // Generated, Sent, Paid, etc.

        [MaxLength(500)]
        public string Notes { get; set; }
    }
}
