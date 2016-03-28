using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace RDotNet.ClrProxy.Resources
{
    public static class ResourcesLoader
    {
        static ResourcesLoader()
        {
            LoadOlsonWindowsMapping();
            utcOlsonTimezone = new[] { TimeZoneInfo.Utc.GetOlsonTimezone() };
            localOlsonTimezon = new[] { TimeZoneInfo.Local.GetOlsonTimezone() };
        }

        #region Timezones

        private static Dictionary<string, TimeZoneInfo> olsonToWindows;
        private static Dictionary<TimeZoneInfo, string> windowsToOlson; 

        private static void LoadOlsonWindowsMapping()
        {
            var olsonWindowsMapping = LoadOlsonWindowsMappingTimeZones();
            var windowsTz = TimeZoneInfo.GetSystemTimeZones().ToDictionary(p => p.Id, p => p);

            olsonToWindows = new Dictionary<string, TimeZoneInfo>();
            windowsToOlson = new Dictionary<TimeZoneInfo, string>();

            var length = olsonWindowsMapping.Length;
            for (var i = 0; i < length; i++)
            {
                if (olsonWindowsMapping[i].Type == null) continue;

                var timezone = windowsTz[olsonWindowsMapping[i].Other];

                var type = olsonWindowsMapping[i].Type;
                if (string.Equals("001", olsonWindowsMapping[i].Territory))
                    windowsToOlson[timezone] = type;

                olsonToWindows[type] = timezone;
            }

            olsonToWindows["CET"] = windowsTz["Central Europe Standard Time"];
            olsonToWindows["CEST"] = windowsTz["Central Europe Standard Time"];
            olsonToWindows["EET"] = windowsTz["E. Europe Standard Time"];
            olsonToWindows["EST"] = windowsTz["E. Europe Standard Time"];
            olsonToWindows["WET"] = windowsTz["W. Europe Standard Time"];
            olsonToWindows["EDT"] = windowsTz["US Eastern Standard Time"];
        }

        private static OlsonWindowsMapItem[] LoadOlsonWindowsMappingTimeZones()
        {
            string xml;
            using (var stream = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetManifestResourceStream("RDotNet.ClrProxy.Resources.WindowsZones.xml"))
            {
                using (var reader = new StreamReader(stream))
                {
                    xml = reader.ReadToEnd();
                }
            }

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var nodes = doc.SelectNodes("//*/mapTimezones/mapZone");
            var count = nodes == null ? 0 : nodes.Count;
            var mapZones = new OlsonWindowsMapItem[count];
            for (var i = 0; i < count; i++)
            {
                if (nodes[i].Attributes == null)
                    continue;

                var other = nodes[i].Attributes["other"].Value;
                var territory = nodes[i].Attributes["territory"].Value;

                var type = nodes[i].Attributes["type"].Value;
                var subTypes = type.Split(' ');
                foreach (var subType in subTypes)
                {
                    mapZones[i] = new OlsonWindowsMapItem
                    {
                        Other = other,
                        Type = subType,
                        Territory = territory
                    };
                }
            }

            return mapZones;
        }

        public class OlsonWindowsMapItem
        {
            public string Other { get; set; }
            public string Territory { get; set; }
            public string Type { get; set; }

            public override string ToString()
            {
                return string.Format("Other: {0}, Territory: {1}, Type: {2}", Other, Territory, Type);
            }
        }

        public static TimeZoneInfo GetWindowsTimezone(this string tzone)
        {
            if (string.IsNullOrEmpty(tzone))
                return TimeZoneInfo.Local;

            TimeZoneInfo timezone;
            if (olsonToWindows.TryGetValue(tzone, out timezone))
                return timezone;

            return TimeZoneInfo.Local;
        }

        public static string GetOlsonTimezone(this TimeZoneInfo timezone)
        {
            if (timezone == null) return "UTC";

            string tzone;
            if (windowsToOlson.TryGetValue(timezone, out tzone))
                return tzone ?? "UTC";

            return "UTC";
        }

        private static readonly DateTime origin = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime[] FromTick(this double[] ticks, TimeZoneInfo timezone)
        {
            var length = ticks.Length;
            var result = new DateTime[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = TimeZoneInfo.ConvertTime(origin.AddSeconds(ticks[i]), timezone);
            }
            return result;
        }

        public static DateTime[,] FromTicks(this double[,] ticks, TimeZoneInfo timezone)
        {
            var nrow = ticks.GetLength(0);
            var ncol = ticks.GetLength(1);

            var result = new DateTime[nrow, ncol];
            for (var i = 0; i < nrow; i++)
            {
                for (var j = 0; j < ncol; j++)
                    result[i, j] = TimeZoneInfo.ConvertTime(origin.AddSeconds(ticks[i, j]), timezone);
            }
            return result;
        }

        private static readonly string[] utcOlsonTimezone;
        private static readonly string[] localOlsonTimezon;

        public static double ToTicks(this DateTime dateTime, out string[] tzone)
        {
            tzone = dateTime.Kind == DateTimeKind.Utc
                ? utcOlsonTimezone
                : localOlsonTimezon;
            return (dateTime.ToUniversalTime() - origin).TotalSeconds;
        }

        public static double[] ToTicks(this DateTime[] array, out string[] tzone)
        {
            var length = array.Length;
            var result = new double[length];

            tzone = length > 0 && array[0].Kind == DateTimeKind.Utc
                ? utcOlsonTimezone
                : localOlsonTimezon;

            for (var i = 0; i < length; i++)
                result[i] = (array[i].ToUniversalTime() - origin).TotalSeconds;

            return result;
        }

        public static double[,] ToTicks(this DateTime[,] matrix, out string[] tzone)
        {
            var nrow = matrix.GetLength(0);
            var ncol = matrix.GetLength(1);
            var result = new double[nrow,ncol];

            tzone = nrow > 0 && ncol > 0 && matrix[0, 0].Kind == DateTimeKind.Utc
                ? utcOlsonTimezone
                : localOlsonTimezon;

            for (var i = 0; i < nrow; i++)
            {
                for (var j = 0; j < ncol; j++)
                    result[i, j] = (matrix[i, j] - origin).TotalSeconds;
            }

            return result;
        }

        #endregion
    }
}
