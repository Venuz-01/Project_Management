using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelForPMS
{
    public class Holiday
    {
        public int HolidayId { get; set; }
        public DateOnly Date { get; set; }
        public string Description { get; set; }
    }
}
