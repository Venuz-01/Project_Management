using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ModelForPMS
{
    public class Project

    {

        [Key]

        public int ProjectId { get; set; }

        public required string ProjectName { get; set; }

        public int ClientId { get; set; }

        [JsonIgnore]

        public Client? Client { get; set; }

        public required string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? DailyRate { get; set; }

        [JsonIgnore]

        public ICollection<ProjectAssignment>? ProjectAssignments { get; set; }

        // public int GetBudget()

        // {

        //     if (StartDate.HasValue && EndDate.HasValue)

        //     {

        //         // Calculate total days between start and end date for 

        //         int totalDays = (EndDate.Value.DayNumber - StartDate.Value.DayNumber)+1;

        //         // Calculate worked days based on allocation percentage

        //         return (int)(totalDays * DailyRate);

        //     }

        //     return 0;

        // }


    }
}
