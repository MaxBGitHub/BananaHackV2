using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananaHackV2
{
    [Flags]
    internal enum ShiftType{
        Empty   = 0,
        None    = 1 << 1,
        Morning = 1 << 2,
        Late    = 1 << 3,
        OnCall  = 1 << 4,
        School  = 1 << 5,
    }
}
