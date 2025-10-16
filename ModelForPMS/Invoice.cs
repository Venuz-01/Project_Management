using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelForPMS
{
    public class Invoice
    {
       
            public int Invoice_Id { get; set; }
            public int EmployeeId { get; set; }
            public Employee Employee { get; set; }
            public int ProjectId { get; set; }
            public Project Project { get; set; }
            public DateTime Date { get; set; }
            public decimal Days { get; set; }
            public decimal RatePerDay { get; set; } 

    }
}
