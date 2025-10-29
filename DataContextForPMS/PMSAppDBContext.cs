using Microsoft.EntityFrameworkCore;
using ModelForPMS;

namespace DataContextForPMS
{
    public class PMSAppDBContext : DbContext
    {

        public PMSAppDBContext(DbContextOptions<PMSAppDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<EmployeeRole> EmployeeRoles { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectAssignment> ProjectAssignments { get; set; }

        public DbSet<Leave> Leaves { get; set; }

       // public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Holiday> Holidays { get; set; }


    }
}
