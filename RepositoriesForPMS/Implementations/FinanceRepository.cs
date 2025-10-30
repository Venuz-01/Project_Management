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
        // Start with the base query
        IQueryable<ProjectAssignment> query = _context.ProjectAssignments;

        // Apply project filter
        if (projectIdFilter.HasValue)
        {
            query = query.Where(pa => pa.ProjectId == projectIdFilter.Value);
        }

        return await query
            .Select(pa => new AssignmentSummaryDto
            {
                // --- Standard Mappings (Assumed working) ---
                EmployeeFirstName = pa.Employee!.FirstName,
                RoleName = pa.Role!.RoleName,
                ProjectName = pa.Project!.ProjectName,
                ClientName = pa.Project!.Client!.ClientName,
                ClientEmail = pa.Project!.Client!.ClientEmail,

                AllocationPercent = pa.AllocationPercent,
                EmployeeStartDate = pa.StartDate,
                EmployeeEndDate = pa.EndDate,
                WorkedDays = pa.GetWorkedDays(),

                RatePerDay = pa.Project!.DailyRate,
                ProjectStartDate = pa.Project!.StartDate,
                ProjectEndDate = pa.Project!.EndDate,
                //Budget = pa.Project!.GetBudget(),

                // ----------------------------------------------------
                // 1. Calculate Holiday Count using a Correlated Subquery
                // Filters the global Holidays table by the assignment's start/end dates.
                // Note: We use the ternary operator to handle null dates safely.
                HolidayCount = pa.StartDate.HasValue && pa.EndDate.HasValue
                    ? _context.Holidays.Count(h =>
                        h.Date >= pa.StartDate.Value &&
                        h.Date <= pa.EndDate.Value)
                    : 0,

                // 2. Calculate Leave Count using a Correlated Subquery
                // Filters the Leaves table by EmployeeId AND the assignment's date range.
                LeaveCount = pa.StartDate.HasValue && pa.EndDate.HasValue
                    ? _context.Leaves.Count(l =>
                        l.EmployeeId == pa.EmployeeId && // Filter by the assigned Employee
                        l.FromDate <= pa.EndDate.Value && // Leave starts before assignment ends
                        l.ToDate >= pa.StartDate.Value)   // Leave ends after assignment starts (Overlap condition)
                    : 0
                // ----------------------------------------------------
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
