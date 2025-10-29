using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ModelForPMS
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public string ClientEmail { get; set; }  // Matches frontend field

        [JsonIgnore]
        public ICollection<Project>? Projects { get; set; }
    }
}
