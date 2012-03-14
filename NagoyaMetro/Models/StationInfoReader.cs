using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NagoyaMetro.Models
{
    public class StationInfoReader
    {
        static string stationsXmlFile = "stations.xml";
        IEnumerable<StationDescription> stationDescriptions;

        public StationInfoReader()
        {
            using (var reader = XmlReader.Create(stationsXmlFile))
            {
                stationDescriptions =                XDocument.Load(reader)
                    .Descendants("StationInfo")
                    .Select(x => new StationDescription()
                    {
                        Name = x.Attribute("Name").Value,
                        Reading = x.Attribute("Reading").Value,
                        IsBusStop = Boolean.Parse(x.Attribute("IsBusStop").Value)
                    })
                    .ToArray();
            }
        }

        /// <summary>
        /// 指定したクエリキーワードを含む駅名・バス停名及びそれぞれの読みを取得します。
        /// </summary>
        public IEnumerable<StationDescription> QueryStation(string partial)
        {
            return
                this.stationDescriptions
                .Where(x =>
                    x.Name.StartsWith(partial, StringComparison.CurrentCultureIgnoreCase) ||
                    x.Reading.StartsWith(partial, StringComparison.CurrentCultureIgnoreCase))
                .Concat(this.stationDescriptions
                .Where(x =>
                    x.Name.IndexOf(partial, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                    x.Reading.IndexOf(partial, StringComparison.CurrentCultureIgnoreCase) >= 0)
                .Where(x =>
                    !x.Name.StartsWith(partial, StringComparison.CurrentCultureIgnoreCase) &&
                    !x.Reading.StartsWith(partial, StringComparison.CurrentCultureIgnoreCase)));
        }
    }

    public class StationDescription
    {
        public string Name { get; set; }

        public string Reading { get; set; }

        public bool IsBusStop { get; set; }

        public override bool Equals(object obj)
        {
            // 名前が違えばだいたいかぶってない
            var sd = obj as StationDescription;
            return sd != null &&
                sd.Name == this.Name;
        }

        public override int GetHashCode()
        {
            // 名前が違えばだいたいかぶってない
            return Name.GetHashCode();
        }
    }
}