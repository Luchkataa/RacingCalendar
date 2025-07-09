using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RacingCalendar.Data.Models
{
    public class Race
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int SeriesId { get; set; }
        public Series Series { get; set; }

        [Required]
        public int CircuitId { get; set; }
        public Circuit Circuit { get; set; }
        public ICollection<DriverRace> DriverEntries { get; set; } = new List<DriverRace>();
    }
}
