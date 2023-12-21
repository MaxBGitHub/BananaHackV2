using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;


namespace BananaHackV2
{
    internal class IcsBuilder
    {
        private NameValueCollection _fieldValues = new NameValueCollection();

        private const string DATEFORMATUTC  = "yyyyMMddTHHmmss";
        private const string DATEFORMAT     = "yyyyMMdd";

        private const int MAX_LINELENGTH = 75;

        private readonly string PRODID = $"-//MaxB//{nameof(BananaHackV2)}//{nameof(IcsBuilder)}//DE";
        private readonly string UIDTEMPLATE = "{0}-@{1}-EVENT-{2}";


        private string _summary;
        public string Summary 
        { 
            get { 
                return _summary; 
            } 
            set {
                SetSummary(value);
            }
        }

        private string _location;
        public string Location
        {
            get {
                return _location;
            }
            set {
                SetLocation(value);
            }
        }

        private string _description;
        public string Description
        {
            get {
                return _description;
            }
            set {
                SetDescription(value);
            }
        }

        private iCalPriority _priority;
        public iCalPriority Priority
        {
            get {
                return _priority;
            }
            set {
                _priority = value;
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get {
                return _startDate;
            }
            set {
                if (value > _endDate) {
                    var temp = EndDate;
                    EndDate = value;
                    _startDate = temp;
                }
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get {
                return _endDate;
            }
            set {
                if (value < StartDate) {
                    var temp = StartDate;
                    StartDate = value;
                    _endDate = temp;
                }
            }
        }


        private string NormalizeString(string field, string value)
        {
            int nDelim = ":".Length;
            int nField = field.Length;
            int nValue = value.Length;
            int nTotal = nDelim + nField + nValue;

            if (nTotal <= MAX_LINELENGTH) {
                return value;
            }

            int partsRequired = (int)Math.Ceiling((float)nTotal / (float)MAX_LINELENGTH);
            string firstPart = value.Substring(0, MAX_LINELENGTH - 1 - nField - nDelim);
            value = value.Substring(firstPart.Length, value.Length - firstPart.Length);
            List<string> parts = new List<string>();
            parts.Add(firstPart);

            int partCount = 1;
            for (int i = partCount; i < partsRequired; i++) {
                int len = value.Length;
                if (len >= MAX_LINELENGTH) {
                    len = MAX_LINELENGTH - 1;
                }
                string part = value.Substring(0, len);
                parts.Add(part);
                value = value.Substring(len-1, value.Length - len);
            }

            const string HTAB   = "\u0009";
            const string CR     = "\u000d";
            const string LF     = "\u000a";

            string result = parts[0];
            for (int i = 1; i < parts.Count; i++) {
                result += CR + LF + HTAB + parts[i];
            }
            return result;
        }


        private void SetSummary(string summary)
        {
            this._summary = NormalizeString(iCalFieldDescriptor.FIELD_SUMMARY, summary);
        }


        private void SetLocation(string location)
        {
            this._location = NormalizeString(iCalFieldDescriptor.FIELD_LOCATION, location);
        }


        private void SetDescription(string description)
        {
            this._description = NormalizeString(iCalFieldDescriptor.FIELD_DESCRIPTION, description);
        }


        private string GetLocalOffsetGmt()
        {
            var offset = DateTimeOffset.Now.Offset;
            return "+" + offset.Hours.ToString("00") + offset.Minutes.ToString("00");
        }


        private string GetLocalOffsetGmtDaylight()
        {
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo zone = TimeZoneInfo.Local;
            var rules = zone.GetAdjustmentRules().FirstOrDefault();
            if (rules == null) {
                return "+0000";
            }

            var offset = rules.DaylightDelta;
            if (StartDate.IsDaylightSavingTime()) {
                return "+" + offset.Hours.ToString("00") + offset.Minutes.ToString("00");
            }
            else {
                return "+0000";
            }
        }


        private string GetStartDate()
        {
            return StartDate.Date.ToString(DATEFORMAT);
        }


        private string GetEndDate()
        {
            return EndDate.Date.ToString(DATEFORMAT);
        }


        private string GetUID()
        {
            string uid = string.Format(UIDTEMPLATE, DateTime.Now.ToString(DATEFORMATUTC), nameof(IcsBuilder), Guid.NewGuid());
            string temp = iCalFieldDescriptor.FIELD_UID + ":" + uid;
            if (temp.Length >= 74) {
                int descriptorLen = iCalFieldDescriptor.FIELD_UID.Length + ":".Length;
                int pos = 74 - descriptorLen;
                string p1 = temp.Substring(descriptorLen, pos);
                string p2 = temp.Substring(pos, temp.Length - pos);
                uid = p1 + "\r\n" + '\u0009' + p2;
            }
            return uid;
        }


        public string BuildIcalItem()
        {            

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(iCalFieldDescriptor.FIELD_BEGIN + ":" + iCalFieldDescriptor.VALUE_VCALENDAR);
            sb.AppendLine(iCalFieldDescriptor.FIELD_VERSION + ":" + iCalFieldDescriptor.VALUE_VERSION_NUMBER);
            sb.AppendLine(iCalFieldDescriptor.FIELD_PRODID + ":" +  PRODID);
            sb.AppendLine(iCalFieldDescriptor.FIELD_CALSCALE + ":" + iCalFieldDescriptor.VALUE_CALSCALE_GREGORIAN);
            sb.AppendLine(iCalFieldDescriptor.FIELD_METHOD + ":" + iCalFieldDescriptor.VALUE_METHOD_PUBLISH);
            sb.AppendLine(iCalFieldDescriptor.FIELD_BEGIN + ":" + iCalFieldDescriptor.VALUE_VTIMEZONE);
            sb.AppendLine(iCalFieldDescriptor.FIELD_TZID + ":" + iCalFieldDescriptor.VALUE_TZID_EUROPE_BERLIN);
            sb.AppendLine(iCalFieldDescriptor.FIELD_TZURL + ":" + iCalFieldDescriptor.VALUE_TZURL_EUROPE_BERLIN);
            sb.AppendLine(iCalFieldDescriptor.FIELD_BEGIN + ":" + iCalFieldDescriptor.VALUE_DAYLIGHT);
            sb.AppendLine(iCalFieldDescriptor.FIELD_TZOFFSETFROM + ":" + GetLocalOffsetGmtDaylight());
            sb.AppendLine(iCalFieldDescriptor.FIELD_TZOFFSETTO + ":" + GetLocalOffsetGmt());
            sb.AppendLine(iCalFieldDescriptor.FIELD_END + ":" + iCalFieldDescriptor.VALUE_DAYLIGHT);
            sb.AppendLine(iCalFieldDescriptor.FIELD_BEGIN + ":" + iCalFieldDescriptor.VALUE_STANDARD);
            sb.AppendLine(iCalFieldDescriptor.FIELD_TZOFFSETFROM + ":" + "+0000");
            sb.AppendLine(iCalFieldDescriptor.FIELD_TZOFFSETTO + ":" + GetLocalOffsetGmt());
            sb.AppendLine(iCalFieldDescriptor.FIELD_END + ":" + iCalFieldDescriptor.VALUE_STANDARD);
            sb.AppendLine(iCalFieldDescriptor.FIELD_END + ":" + iCalFieldDescriptor.VALUE_VTIMEZONE);
            sb.AppendLine(iCalFieldDescriptor.FIELD_BEGIN + ":" + iCalFieldDescriptor.VALUE_VEVENT);
            sb.AppendLine(iCalFieldDescriptor.FIELD_DTSTAMP + ":" + DateTime.Now.ToString(DATEFORMATUTC));
            sb.AppendLine(iCalFieldDescriptor.FIELD_UID + ":" + GetUID());
            sb.AppendLine(iCalFieldDescriptor.FIELD_DTCREATED + ":" + DateTime.Now.ToString(DATEFORMATUTC));
            sb.AppendLine(iCalFieldDescriptor.FIELD_SUMMARY + ":" + Summary);
            sb.AppendLine(iCalFieldDescriptor.FIELD_DESCRIPTION + ":" + Description);
            sb.AppendLine(iCalFieldDescriptor.FIELD_PRIOARITY + ":" + (int)Priority);
            sb.AppendLine(iCalFieldDescriptor.FIELD_DTSTART + ":" + GetStartDate());
            sb.AppendLine(iCalFieldDescriptor.FIELD_DTEND + ":" + GetEndDate());
            sb.AppendLine(iCalFieldDescriptor.FIELD_END + ":" + iCalFieldDescriptor.VALUE_VEVENT);
            sb.AppendLine(iCalFieldDescriptor.FIELD_END + ":" + iCalFieldDescriptor.VALUE_VCALENDAR);
            return sb.ToString();
        }

        //public void CreateEvent();


        private bool TryGetPriority(int priority, out iCalPriority calPriority)
        {
            if ((priority < (int)iCalPriority._0)
                || (priority > (int)iCalPriority._9))
            {
                calPriority = iCalPriority.Low;
                return false;
            }

            calPriority = (iCalPriority)priority;
            return true;
        }


        public IcsBuilder(
            string summary, 
            string location,
            string description,
            iCalPriority priority,
            DateTime start, 
            DateTime end)
        {
            SetSummary(summary);
            SetLocation(location);
            SetDescription(description);
            Priority    = priority;
            StartDate   = start;
            EndDate     = end;
        }


        public IcsBuilder(
            string summary, 
            string location, 
            string description, 
            int priority, 
            DateTime start,
            DateTime end) 
        {            
            if (!(TryGetPriority(priority, out _priority))) {
                throw new ArgumentOutOfRangeException(
                    "Invalid priority, valid values are in the range 0 to 9.");
            }

            SetSummary(summary);
            SetLocation(location);
            SetDescription(description);
            StartDate   = start;
            EndDate     = end;
        }


        public IcsBuilder()
        {
        }


        private static class iCalFieldDescriptor
        {
            public const string VALUE_VERSION_NUMBER        = "2.0";
            public const string VALUE_CALSCALE_GREGORIAN    = "GREGORIAN";
            public const string VALUE_METHOD_PUBLISH        = "PUBLISH";
            public const string VALUE_VCALENDAR             = "VCALENDAR";
            public const string VALUE_VTIMEZONE             = "VTIMEZONE";
            public const string VALUE_VEVENT                = "VEVENT";
            public const string VALUE_STANDARD              = "STANDARD";
            public const string VALUE_TZID_EUROPE_BERLIN    = "Europe/Berlin";
            public const string VALUE_TZURL_EUROPE_BERLIN   = "http://tzurl.org/zoneinfo/Europe/Berlin";
            public const string VALUE_DAYLIGHT              = "DAYLIGHT";

            public const string FIELD_END           = "END";
            public const string FIELD_BEGIN         = "BEGIN";
            public const string FIELD_VERSION       = "VERSION";
            public const string FIELD_PRODID        = "PRODID";
            public const string FIELD_UID           = "UID";
            public const string FIELD_CALSCALE      = "CALSCALE";
            public const string FIELD_METHOD        = "METHOD";
            public const string FIELD_TZID          = "TZID";
            public const string FIELD_TZURL         = "TZURL";
            public const string FIELD_TZOFFSETTO    = "TZOFFSETTO";
            public const string FIELD_TZOFFSETFROM  = "TZOFFSETFROM";
            public const string FIELD_DTSTAMP       = "DTSTAMP";
            public const string FIELD_DTCREATED     = "DTCREATED";
            public const string FIELD_DTSTART       = "DTSTART;VALUE=DATE";
            public const string FIELD_DTEND         = "DTEND;VALUE=DATE";
            public const string FIELD_SUMMARY       = "SUMMARY";
            public const string FIELD_LOCATION      = "LOCATION";
            public const string FIELD_DESCRIPTION   = "DESCRIPTION";
            public const string FIELD_PRIOARITY     = "PRIORITY";           
            
        }
    }
}
