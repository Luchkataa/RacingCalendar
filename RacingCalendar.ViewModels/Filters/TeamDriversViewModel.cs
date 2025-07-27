using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingCalendar.ViewModels.Filters
{
    public class TeamDriversViewModel
    {
        public string TeamName { get; set; } = null!;
        public List<DriverViewModel> Drivers { get; set; } = new();
    }
}
