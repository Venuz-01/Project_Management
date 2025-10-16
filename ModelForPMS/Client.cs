using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelForPMS
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public ICollection<Project> Projects { get; set; }
    }
}
