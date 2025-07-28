using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingCalendar.ViewModels.Filters
{
    public class CircuitListViewModel
    {
        public PaginatedList<CircuitViewModel> Circuits { get; set; }
        public string? CountryFilter { get; set; }
        public string? SortOrder { get; set; }
        public List<string> CountryOptions { get; set; } = new();
    }
}
