using System.ComponentModel.DataAnnotations;

namespace RacingCalendar.ViewModels
{
    public class TeamViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }
    }
}
