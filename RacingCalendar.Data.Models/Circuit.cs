using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingCalendar.Data.Models
{
    public class Circuit
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Url]
        [Display(Name = "Layout Image URL")]
        public string? LayoutImageUrl { get; set; }

        public ICollection<Race> Races { get; set; } = new List<Race>();
    }
}
