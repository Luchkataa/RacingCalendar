using System.ComponentModel.DataAnnotations;

namespace RacingCalendar.ViewModels
{
    public class CircuitViewModel
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
    }
}
