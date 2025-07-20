using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RacingCalendar.ViewModels
{
    public class DriverViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public string Nationality { get; set; }

        [Display(Name = "Team")]
        public int? TeamId { get; set; }

        public string? TeamName { get; set; }

        [Url]
        [Display(Name = "Image URL")]
        public string? DriverImageUrl { get; set; }

        public IEnumerable<SelectListItem> Teams { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
