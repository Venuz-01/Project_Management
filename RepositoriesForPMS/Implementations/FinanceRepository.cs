using DataContextForPMS;
using Microsoft.EntityFrameworkCore;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;
using ModelForPMS.ModelDtos;

public class FinanceRepository : IFinanceRepository
{
    private readonly PMSAppDBContext _context;

    public FinanceRepository(PMSAppDBContext context)
    {
        _context = context;
    }

    public async Task<List<AssignmentSummaryDto>> GetAssignmentSummaryAsync(int? projectIdFilter = null)
    {
        // 1. Start query from the ProjectAssignments DbSet
        IQueryable<ProjectAssignment> query = _context.ProjectAssignments;

        // 2. If a filter is provided, apply it here
        if (projectIdFilter.HasValue)
        {
            query = query.Where(pa => pa.ProjectId == projectIdFilter.Value);
        }

        // 3. Eagerly load all required related entities in a single trip.
        //    (This step is crucial for performance and correctly populating the .Select() logic)
        query = query
            .Include(pa => pa.Employee) // Load the related Employee object
            .Include(pa => pa.Role)     // Load the related Role object
            .Include(pa => pa.Project); // Load the related Project object

        // 4. Project the result into the lightweight DTO
        //    EF Core translates this into a single SQL query with JOINs.
        return await query
            .Select(pa => new AssignmentSummaryDto
            {
                AssignmentId = pa.AssignmentId,

                // From Employee
                EmployeeFirstName = pa.Employee.FirstName,

                // From Role
                RoleName = pa.Role.RoleName,

                // From Project
                ProjectName = pa.Project.ProjectName,

                // From ProjectAssignment (the base table)
                StartDate = pa.StartDate,
                EndDate = pa.EndDate
            })
            .ToListAsync();
    }

    public async Task<List<Project>> GetAllProjectsAsync()
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.ProjectAssignments)
                .ThenInclude(pa => pa.Employee)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectWithDetailsAsync(int projectId)
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.ProjectAssignments)
                .ThenInclude(pa => pa.Employee)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);
    }

    public async Task<int> GetWorkingDaysAsync(DateTime start, DateTime end)
    {
        var holidays = await _context.Holidays
            .Where(h => h.Date >= DateOnly.FromDateTime(start) && h.Date <= DateOnly.FromDateTime(end))
            .Select(h => h.Date)
            .ToListAsync();

        int totalDays = (end - start).Days + 1;
        int workingDays = Enumerable.Range(0, totalDays)
            .Select(i => DateOnly.FromDateTime(start.AddDays(i)))
            .Count(d => d.DayOfWeek != DayOfWeek.Saturday &&
                        d.DayOfWeek != DayOfWeek.Sunday &&
                        !holidays.Contains(d));

        return workingDays;
    }

    //public async Task<List<Invoice>> GenerateInvoicesForProjectAsync(int projectId)
    //{
    //    var project = await GetProjectWithDetailsAsync(projectId);
    //    if (project == null) return new List<Invoice>();

    //    int workingDays = await GetWorkingDaysAsync(project.StartDate, project.EndDate);
    //    var invoices = new List<Invoice>();

    //    foreach (var assignment in project.ProjectAssignments)
    //    {
    //        var employee = assignment.Employee;

    //        var leaves = await _context.Leaves
    //            .Where(l => l.EmployeeId == employee.EmployeeId &&
    //                        l.ProjectId == projectId &&
    //                        l.FromDate >= DateOnly.FromDateTime(project.StartDate) &&
    //                        l.ToDate <= DateOnly.FromDateTime(project.EndDate))
    //            .ToListAsync();

    //        int leaveDays = leaves.Sum(l => (l.ToDate.DayNumber - l.FromDate.DayNumber + 1));
    //        int workedDays = workingDays - leaveDays;

    //        invoices.Add(new Invoice
    //        {
    //            InvoiceId = 0,
    //            Date = DateTime.Now,
    //            EmployeeId = employee.EmployeeId,
    //            EmployeeName = $"{employee.FirstName} {employee.LastName}",
    //            ProjectId = project.ProjectId,
    //            ProjectName = project.ProjectName,
    //            StartDate = project.StartDate,
    //            EndDate = project.EndDate,
    //            ClientId = project.Client.ClientId,
    //            ClientName = project.Client.ClientName,
    //            WorkedDays = workedDays > 0 ? workedDays : 0,
    //            RatePerDay = project.DailyRate ?? 0,
    //            Budget = (project.DailyRate ?? 0) * (workedDays > 0 ? workedDays : 0)
    //        });
    //    }

    //    return invoices;
    //}
}