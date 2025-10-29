using DataContextForPMS;
using Microsoft.EntityFrameworkCore;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesForPMS.Implementations
{
    public class HRrepository : IHRrepository
    {
        private readonly PMSAppDBContext _context;

        public HRrepository(PMSAppDBContext context)
        {
            _context = context;
        }

        // CREATE


        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            // Includes are often omitted for simple DTO mapping, but added here for completeness
            return await _context.Employees.Include(s => s.Roles)
            .OrderBy(e => e.EmployeeId)
            .ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Employees
                .Include(s => s.Roles)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Employees.AnyAsync(e => e.EmployeeId == employeeId);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task AddLeaveAsync(Leave leave)
        {
            await _context.Leaves.AddAsync(leave);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLeaveAsync(int employeeId)
        {
            var leave = await _context.Leaves.Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.LeaveId) // Sorts latest to earliest
            .FirstOrDefaultAsync();
            if (leave != null)
            {
                _context.Leaves.Remove(leave);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateLeaveAsync(Leave leave)
        {
            _context.Entry(leave).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Leave>> GetAllLeavesAsync()
        {
            return await _context.Leaves
                             .ToListAsync();
        }

        public async Task<List<Leave>> GetAllLeavesWithEmployeeIDAsync(int EmployeeId)
        {
            // The Where method filters the collection to include only leaves 
            // where the EmployeeId matches the input 'id'.
            return await _context.Leaves
                                 .Where(s => s.EmployeeId == EmployeeId)
                                 .ToListAsync();
        }

        public async Task<int> GetAllLeavesCountWithEmployeeIDAsync(int EmployeeId)
        {
            return await _context.Leaves
                             .Where(s => s.EmployeeId == EmployeeId)
                             .CountAsync();
        }
    }
}

