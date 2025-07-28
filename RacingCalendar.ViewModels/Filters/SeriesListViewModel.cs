using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingCalendar.ViewModels.Filters
{
    public class SeriesListViewModel
    {
        public PaginatedList<SeriesViewModel> Series { get; set; }

        public string? SearchTerm { get; set; }
        public string? SortOrder { get; set; }

        public int CurrentPage => Series.PageIndex;
        public int TotalPages => Series.TotalPages;
    }

}
