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
        //    ⚠️ NOTE: For a Projection Query (using .Select()), 
        //    the Include/ThenInclude lines are OPTIONAL as EF Core will generate the JOINs anyway. 
        //    I'm commenting them out to show the most efficient, projection-based approach.
        /*
        query = query
            .Include(pa => pa.Employee) 
            .Include(pa => pa.Role)     
            // To include Client, we would chain ThenInclude off the Project:
            // .Include(pa => pa.Project).ThenInclude(p => p.Client); 
            .Include(pa => pa.Project); 
        */

        // 4. Project the result into the lightweight DTO.
        //    Since ProjectAssignment has a Project navigation property (pa.Project), 
        //    and Project has a Client navigation property (pa.Project.Client), 
        //    we can access the ClientName property by simply chaining the navigations.
        return await query
            .Select(pa => new AssignmentSummaryDto
            {
                // From Employee
                // NOTE: Employee must have a 'FirstName' property for this to compile.
                EmployeeFirstName = pa.Employee!.FirstName,

                // From Role
                // NOTE: Role must have a 'RoleName' property for this to compile.
                RoleName = pa.Role!.RoleName,

                // From Project
                ProjectName = pa.Project!.ProjectName,

                // From Project -> Client (The required step)
                // Access the client via the project navigation property: pa.Project.Client
                ClientName = pa.Project!.Client!.ClientName, // Assuming Client has a 'ClientName' property

                ClientEmail = pa.Project!.Client!.ClientEmail,
                // From ProjectAssignment (the base table)
                EmployeeStartDate = pa.StartDate,
                EmployeeEndDate = pa.EndDate,

                // The .GetWorkedDays() C# method can be called here if it's simple enough 
                // to be translated by EF Core, but it's often safer to calculate it client-side.
                // For this example, we'll keep your original code which works if EF Core supports translation.
                WorkedDays = pa.GetWorkedDays(),

                RatePerDay = pa.Project!.DailyRate,

                ProjectStartDate = pa.Project!.StartDate,

                ProjectEndDate = pa.Project!.EndDate,

                Budget = pa.Project!.GetBudget()
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


}
