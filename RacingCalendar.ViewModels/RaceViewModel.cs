using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RacingCalendar.ViewModels
{
    public class RaceViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Race Date")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Series")]
        public int SeriesId { get; set; }

        [Required]
        [Display(Name = "Circuit")]
        public int CircuitId { get; set; }

        public string? SeriesName { get; set; }
        public string? CircuitName { get; set; }

        public IEnumerable<SelectListItem> SeriesOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> CircuitOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
