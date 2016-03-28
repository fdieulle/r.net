using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using RDotNet.ClrProxy.Resources;

namespace RDotNet.ClrProxyTests
{
    [TestFixture]
    public class DateTimeTests
    {
        [Test]
        public void GenerateMapping()
        {
            Dictionary<string, TimeZoneInfo> d1;
            Dictionary<TimeZoneInfo, string> d2;
            //ResourcesLoader.LoadOlsonWindowsMapping(out d1, out d2);
            var windowsTimeZones = TimeZoneInfo.GetSystemTimeZones();
            foreach(var tz in windowsTimeZones)
                Console.WriteLine(tz.Id);
            var olsonWindowsMapping = LoadOlsonWindowsMappingTimeZones("./Resources/WindowsZones.xml");
            //var rTimezones = LoadRTimeZones();

            var olsonToWindows = new Dictionary<string, TimeZoneInfo>();
            var windowsToOlson = new Dictionary<TimeZoneInfo, string>();

            var length = olsonWindowsMapping.Length;
            for (var i = 0; i < length; i++)
            {
                if (olsonWindowsMapping[i].Type == null) continue;

                var timezone = TimeZoneInfo.FindSystemTimeZoneById(olsonWindowsMapping[i].Other);

                var type = olsonWindowsMapping[i].Type;
                if (string.Equals("001", olsonWindowsMapping[i].Territory))
                    windowsToOlson[timezone] = type;

                olsonToWindows[type] = timezone;   
            }

            //CheckDiff(olsonToWindows, olsonToWindows2, "With rTab", "Without rTab");
            CheckDiff(windowsTimeZones.ToDictionary(p => p, p => p.Id), windowsToOlson, "Win TZ", "Win to Olson");

            var rTzones = GetAllTimeZone();
            var rTzonesDico = rTzones.ToDictionary(p => p, p => (TimeZoneInfo)null);
            var olsonMap = olsonWindowsMapping.Select(p => p.Type).Distinct().ToDictionary(p => p, p => (TimeZoneInfo) null);

            CheckDiff(rTzonesDico, olsonMap, "R", "Olson");
            CheckDiff(olsonMap, rTzonesDico, "Mapping", "R");

        }

        private static OlsonWindowsMapItem[] LoadOlsonWindowsMappingTimeZones(string xmlFilePath)
        {
            var doc = new XmlDocument();
            doc.Load(xmlFilePath);

            var nodes = doc.SelectNodes("//*/mapTimezones/mapZone");
            var count = nodes == null ? 0 : nodes.Count;
            var mapZones = new OlsonWindowsMapItem[count];
            for (var i = 0; i < count; i++)
            {
                if(nodes[i].Attributes == null)
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

        private static List<RTimeZone> LoadRTimeZones()
        {
            var rhome = Environment.GetEnvironmentVariable("R_HOME");
            if (string.IsNullOrEmpty(rhome)) return null;

            var zoneTabFilePath = Path.Combine(rhome, "share\\zoneinfo\\zone.tab");
            if (!File.Exists(zoneTabFilePath)) return null;

            var lines = File.ReadAllLines(zoneTabFilePath);
            var length = lines.Length;
            var list = new List<RTimeZone>(length);
            for (var i = 0; i < length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]) || lines[i].StartsWith("#"))
                    continue;

                var split = lines[i].Split('\t');
                if(split.Length < 3) continue;

                var timezone = new RTimeZone
                {
                    Code = split[0],
                    Coordinates = split[1],
                    Timezone = split[2]
                };

                if (split.Length > 3)
                    timezone.Comments = split[3];

                list.Add(timezone);
            }

            return list;
        }

        public void CheckDiff<TKey, TValue>(
            Dictionary<TKey, TValue> x, 
            Dictionary<TKey, TValue> y,
            string xName = "X",
            string yName = "Y",
            bool display = false)
        {
            var copyX = x.ToDictionary(p => p.Key, p => p.Value);
            foreach (var pair in y)
                copyX.Remove(pair.Key);

            var copyY = y.ToDictionary(p => p.Key, p => p.Value);
            foreach (var pair in x)
                copyY.Remove(pair.Key);



            Console.WriteLine("Remaining in {0} Count: {1}", xName, copyX.Count);
            if (display)
            {
                foreach (var pair in copyX)
                    Console.WriteLine("[{0}] {1}", pair.Key, pair.Value);
            }

            Console.WriteLine("Remaining in {0} Count: {1}", yName, copyY.Count);
            if (display)
            {
                foreach (var pair in copyY)
                    Console.WriteLine("[{0}] {1}", pair.Key, pair.Value);
            }
        }

        private static List<string> GetAllTimeZone()
        {
            var rhome = Environment.GetEnvironmentVariable("R_HOME");
            if (string.IsNullOrEmpty(rhome)) return null;

            var zoneTabFilePath = Path.Combine(rhome, "share\\zoneinfo");
            if (!Directory.Exists(zoneTabFilePath)) return null;

            var tzones = new List<string>();
            FillTZone(tzones, zoneTabFilePath, null);

            return tzones;
        }

        private static void FillTZone(List<string> tzones, string path, string prefix)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (string.IsNullOrEmpty(fileInfo.Extension))
                {
                    var nextPrefix = prefix != null
                        ? string.Format("{0}/{1}", prefix, fileInfo.Name)
                        : fileInfo.Name;
                    tzones.Add(nextPrefix);
                }
            }

            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                var nextPrefix = prefix != null 
                    ? string.Format("{0}/{1}", prefix, directoryInfo.Name) 
                    : directoryInfo.Name;

                FillTZone(tzones, directory, nextPrefix);
            }
        }
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

    public class RTimeZone
    {
        public string Code { get; set; }
        public string Coordinates { get; set; }
        public string Timezone { get; set; }
        public string Comments { get; set; }

        public override string ToString()
        {
            return string.Format("Code: {0}, Coordinates: {1}, Timezone: {2}, Comments: {3}", Code, Coordinates, Timezone, Comments);
        }
    }
}
