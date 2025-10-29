using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelForPMS.ModelDtos
{
    public class InvoiceDetailDto
    {
        public int InvoiceId { get; set; }
        public DateTime GeneratedOn { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }

        public int ClientId { get; set; }
        public string ClientName { get; set; }

        public int WorkedDays { get; set; }
        public decimal RatePerDay { get; set; }
        public decimal Budget => WorkedDays * RatePerDay;
    }
}
