using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingCalendar.Data.Models
{
    public class DriverRace
    {
        public int DriverId { get; set; }
        public Driver Driver { get; set; }

        public int RaceId { get; set; }
        public Race Race { get; set; }

        public int? Position { get; set; }
        public bool DidNotFinish { get; set; }
    }
}
