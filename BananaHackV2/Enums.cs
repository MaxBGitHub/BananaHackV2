using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananaHackV2
{
    internal enum ImportanceIncrement {
        Low     = 0,
        Normal  = 1,
        High    = 2
    }


    internal enum Importance {
        High    = 4,
        Normal  = 3,
        Low     = 2
    }


    [Flags]
    internal enum ShiftType{
        Empty   = 0,
        None    = 1 << 1,
        Morning = 1 << 2,
        Late    = 1 << 3,
        OnCall  = 1 << 4,
        School  = 1 << 5,
    }


    internal enum iCalPriority : byte {
        _0 = 0,
        _1 = 1,
        _2 = 2,
        _3 = 3,
        _4 = 4,
        _5 = 5,
        _6 = 6,
        _7 = 7,
        _8 = 8,
        _9 = 9,
        Undefined   = 0,
        High        = 1,
        Medium      = 5,
        Low         = 6
    }
}
