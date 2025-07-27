using Microsoft.AspNetCore.Mvc.Rendering;

namespace RacingCalendar.ViewModels.Filters
{
    public class DriverListViewModel
    {
        public IEnumerable<DriverViewModel> Drivers { get; set; } = new List<DriverViewModel>();

        public string? SearchTerm { get; set; }
        public string? SortOrder { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
