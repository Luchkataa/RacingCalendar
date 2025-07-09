using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingCalendar.Data.Models
{
    public class Driver
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public string Nationality { get; set; }

        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public ICollection<DriverRace> RaceEntries { get; set; } = new List<DriverRace>();
    }
}
