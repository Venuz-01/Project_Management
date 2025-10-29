using ModelForPMS;

namespace RepositoriesForPMS.Interfaces

{

    public interface IFinanceRepository

    {

        /// <summary>

        /// Get all projects with client and assignment details

        /// </summary>

        Task<List<Project>> GetAllProjectsAsync();

        /// <summary>

        /// Get a specific project with client and assignments

        /// </summary>

        Task<Project?> GetProjectWithDetailsAsync(int projectId);

        /// <summary>

        /// Calculate the number of working days between start and end dates, excluding weekends and holidays

        /// </summary>

        Task<int> GetWorkingDaysAsync(DateTime start, DateTime end);

        /// <summary>

        /// Generate invoices for all employees assigned to a project

        /// </summary>

        //Task<List<>> GenerateInvoicesForProjectAsync(int projectId);

        Task<List<AssignmentSummaryDto>> GetAssignmentSummaryAsync(int? projectIdFilter = null);

    }

}

