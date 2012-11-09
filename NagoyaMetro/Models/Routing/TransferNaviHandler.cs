using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Dulcet.Util;
using Hailstone.Linq;
using Hailstone.Reactive;
using Microsoft.Phone.Reactive;
using Net.Tauchi.Text;

namespace NagoyaMetro.Models.Routing
{
    public static class TransferNaviHandler
    {
        private static readonly string TransferNaviUrl = "http://kotsuk.city.nagoya.jp/route/web/exp.cgi";
        private static readonly int LastFirstTrainHour = 4;
        private static readonly Encoding InternalEncoding = new SjisEncoding();

        /// <summary>
        /// 「なごや乗換案内」にクエリして結果を取得します。
        /// </summary>
        public static IObservable<TransportRoute> QueryTransportInfo(SearchDescription searchDescription)
        {
            try
            {
                Dictionary<string, string> postData = new Dictionary<string, string>();
                // 出発・到着点、経由地
                postData.Add("val_from", searchDescription.Origin);
                postData.Add("val_get_from", searchDescription.Origin);
                postData.Add("val_to", searchDescription.Destination);
                postData.Add("val_get_to", searchDescription.Destination);
                if (!String.IsNullOrEmpty(searchDescription.Via))
                    postData.Add("val_via", searchDescription.Via);

                // 時間
                switch (searchDescription.TimeType)
                {
                    case TimeType.DepartureTime:
                    case TimeType.ArrivalTime:
                        postData.Add("val_hour", searchDescription.Time.Hour.ToString());
                        postData.Add("val_min", searchDescription.Time.Minute.ToString());
                        break;
                    case TimeType.FirstTrain:
                    case TimeType.LastTrain:
                        postData.Add("val_hour", LastFirstTrainHour.ToString());
                        postData.Add("val_min", "0");
                        break;
                }

                // 時間種別
                switch (searchDescription.TimeType)
                {
                    case TimeType.DepartureTime:
                    case TimeType.FirstTrain:
                        postData.Add("val_timekb", "DEP");
                        break;
                    case TimeType.ArrivalTime:
                    case TimeType.LastTrain:
                        postData.Add("val_timekb", "ARR");
                        break;
                }

                // 乗り換えキー
                switch (searchDescription.PreferKey)
                {
                    case PreferKey.Cheaper:
                        postData.Add("val_sort", "5");
                        break;
                    case PreferKey.LesserTransfer:
                        postData.Add("val_sort", "2");
                        break;
                }

                // バスのみ？
                if (searchDescription.IsSearchOnlyBus)
                    postData.Add("val_bus_only", "1");

                // さいごに
                postData.Add("val_htmb", "result");

                // ポストデータ組み立て
                var postbyte =
                    InternalEncoding.GetBytes(
                        postData.Select(kv =>
                            kv.Key + "=" + HttpUtil.UrlEncode(kv.Value, InternalEncoding))
                        .JoinString("&"));

                System.Diagnostics.Debug.WriteLine(postData.Select(kv =>
                            kv.Key + "=" + HttpUtility.UrlEncode(kv.Value))
                        .JoinString("&"));
                var req = HttpWebRequest.CreateHttp(TransferNaviUrl);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                return req
                    .UploadDataAsync(postbyte)
                    .Select(s =>
                    {
                        using (var sr = new StreamReader(s.GetResponseStream(), InternalEncoding))
                        {
                            return sr.ReadToEnd();
                        }
                    })
                    .Parse();
            }
            catch (Exception ex)
            {
                return Observable.Throw<TransportRoute>(ex);
            }
        }

        private static IObservable<TransportRoute> Parse(this IObservable<string> source)
        {
            return source
                .SelectMany(s => ParseInternal(s));
        }

        private static Regex GroupingRegex = new Regex("<table class=\"paint2\".*?>(.*?)</table>[\r\n\t ]*<table.*?>(.*?)</table>",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static Regex RouteNameRegex = new Regex("<td class=\"r_d_header1\".*?>(.*?)</td>",
            RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex TimeAndDistanceRegex = new Regex("<td class=\"r_d_header2\".*?>.*?<font class=\"bold\">(.*?)<br.*?>.*?<font class=\"bold\">(.*?)</td>",
            RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex FareAndTransferRegex = new Regex("<td class=\"r_d_header3\".*?>.*?<font class=\"bold\">(.*?)<br.*?>.*?<font class=\"bold\">(.*?)</td>",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static Regex TableRowRegex = new Regex("<tr>(.*?)</tr>",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static Regex TransferPointTypeRegex = new Regex("alt=\"(.*?)\"",
            RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex TransferPointNameRegex = new Regex("<td colspan=\"2\">(.*?)<",
            RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex ThruTransferPointNameRegex = new Regex("<td colspan=\"1\">(.*?)<",
            RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex LineColorRegex = new Regex("color=\"#(.*?)\"",
            RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex LineNameRegex = new Regex("class=\"route_railname\".*?>(.*?)<",
            RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex DepartureTimeRegex = new Regex("class=\"route_info\">(.*?)<",
            RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex ArrivalTimeRegex = new Regex("br>([^(<br>]*?)</td>",
            RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex FareRegex = new Regex("class=\"route_info2\".*?>(.*?)<",
            RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex DetailRegex = new Regex("<font class=\"route_railname\">.*?</font>\\((.*?)\\)(.*?)<br",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static Regex Tags = new Regex("<.*?>",
            RegexOptions.Singleline | RegexOptions.Compiled);


        private static IEnumerable<TransportRoute> ParseInternal(string data)
        {
            return GroupingRegex.Matches(data)
                .Cast<Match>()
                .Select(m => new
                {
                    description = m.Groups[1].Value,
                    detail = m.Groups[2].Value,
                    result = new TransportRoute()
                })
                .Do(m => m.result.Name = RouteNameRegex.Match(m.description).Groups[1].Value)
                .Do(m =>
                {
                    var tadms = TimeAndDistanceRegex.Match(m.description);
                    var tad = new[] { tadms.Groups[1].Value, tadms.Groups[2].Value }
                        .Select(s => Tags.Replace(s, "")).ToArray();
                    m.result.TotalTime = tad[0];
                    m.result.Distance = tad[1];
                })
                .Do(m =>
                {
                    var fatms = FareAndTransferRegex.Match(m.description);
                    var fat = new[] { fatms.Groups[1].Value, fatms.Groups[2].Value }
                        .Select(s => Tags.Replace(s, "")).ToArray();
                    m.result.Fare = fat[0];
                    m.result.Transfer = fat[1];
                })
                .Do(m => m.result.TransportInfo = GenerateTransportInfo(m.detail).ToArray())
                .Do(m => m.result.TransportInfo.Aggregate((bt, nt) =>
                {
                    bt.Destination = nt.Origin;
                    return nt;
                }))
                .Do(m => m.result.TransportInfo = m.result.TransportInfo
                    .Take(m.result.TransportInfo.Count() - 1).ToArray()) // 末尾をぶった切る
                .Select(m => m.result);
        }

        private static IEnumerable<TransportInfo> GenerateTransportInfo(string input)
        {
            System.Diagnostics.Debug.WriteLine(TableRowRegex.Matches(input).Cast<Match>().Select(m => m.Groups[1].Value).JoinString(", "));
            return TableRowRegex.Matches(input)
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .Skip(1) // 先頭はヘッダーなので読み飛ばす
                .Block(2) // 2つごとでグループ化
                .Select(s =>
                {
                    var t = s.Take(2).ToArray();
                    return GetTransportInfo(t[0], t[1]);
                });
        }

        private static TransportInfo GetTransportInfo(string firstRow, string secondRow)
        {
            var ti = new TransportInfo();
            ti.Origin = TransferPointNameRegex.Match(firstRow).Groups[1].Value;
            if (String.IsNullOrEmpty(ti.Origin))
            {
                ti.Origin = ThruTransferPointNameRegex.Match(firstRow).Groups[1].Value;
                ti.IsPassThru = true;
            }
            if (TransferPointTypeRegex.Match(firstRow).Groups[1].Value == "目的地")
            {
                // 目的地
                return ti;
            }
            ti.LineColor = GetColorFromHexString(LineColorRegex.Match(secondRow).Groups[1].Value);
            ti.LineName = LineNameRegex.Match(secondRow).Groups[1].Value;
            ti.DepartureTime = DepartureTimeRegex.Match(secondRow).Groups[1].Value;
            ti.ArrivalTime = ArrivalTimeRegex.Match(secondRow).Groups[1].Value;
            ti.Fare = FareRegex.Match(secondRow).Groups[1].Value;
            var details = DetailRegex.Match(secondRow);
            ti.Detail = Tags.Replace(details.Groups[1].Value + details.Groups[2].Value, "");
            return ti;
        }

        private static Color GetColorFromHexString(string s)
        {
            byte a = System.Convert.ToByte("FF", 16);//Alpha should be 255
            byte r = System.Convert.ToByte(s.Substring(0, 2), 16);
            byte g = System.Convert.ToByte(s.Substring(2, 2), 16);
            byte b = System.Convert.ToByte(s.Substring(4, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }
    }
}