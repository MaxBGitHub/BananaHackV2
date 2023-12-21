using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananaHackV2.OCR
{
    internal struct OcrResult
    {
        public string Month;
        public Dictionary<string, ShiftDayInfo[]> ShiftInfos;

        public OcrResult(string month, Dictionary<string, ShiftDayInfo[]> shiftInfos)
        {
            Month = month;
            ShiftInfos = shiftInfos;
        }
    }
}
