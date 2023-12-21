using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananaHackV2.OCR
{
    internal struct ShiftDayInfo
    {
        public int Day;
        public ShiftType Shifts;

        public ShiftDayInfo(int day, ShiftType shifts)
        {
            this.Day = day;
            this.Shifts = shifts;
        }
    }
}
