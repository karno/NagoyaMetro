using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Hailprism.Spectrum.Linq;
using Hailprism.Spectrum.Mvvm;
using NagoyaMetro.Models;
using NagoyaMetro.Models.Routing;

namespace NagoyaMetro.ViewModels
{
    public class ResultViewModel : ViewModel
    {
        public ResultViewModel()
        {
            if (IsInDesignMode)
            {
                // バインド用ダミーデータ
                var tp = new TransportRoute();
                tp.Name = "経路1";
                tp.TotalTime = "1時間23分";
                tp.Fare = "99999円";
                tp.Transfer = "128回";
                tp.Distance = "2.56km";
                tp.TransportInfo = new[]{
                    new TransportInfo(){
                        LineColor = Colors.Red,
                        LineName = "東山線",
                        Detail = "藤ヶ丘行き",
                        Fare = "920円",
                        Origin = "高畑",
                        Destination = "藤ヶ丘",
                        DepartureTime = "2:56",
                        ArrivalTime = "5:12",
                        IsPassThru = false
                    },
                    new TransportInfo(){
                        LineColor = Colors.Blue,
                        LineName = "東山線",
                        Detail = "高畑行き",
                        Fare = "",
                        Origin = "藤ヶ丘",
                        Destination = "高畑",
                        DepartureTime = "5:12",
                        ArrivalTime = "10:24",
                        IsPassThru = true
                    }
                };
                _routes = new[] { tp }.Select(s => new RouteViewModel(s)).ToArray();
                SearchInfo = "[DUMMY DATA]";
            }
            else
            {
                _routes = Setting.LastResult.Select(r => new RouteViewModel(r)).ToArray();
                SearchInfo = Setting.LastSearchDescription.ToString();
            }
        }

        public string SearchInfo { get; private set; }

        private IEnumerable<RouteViewModel> _routes;
        public IEnumerable<RouteViewModel> Routes
        {
            get { return _routes; }
        }
    }

    public class RouteViewModel : ViewModel
    {
        public RouteViewModel(TransportRoute route)
        {
            this.Name = route.Name;
            this.TotalTime = route.TotalTime;
            this.Fare = route.Fare;
            this.TransferCount = route.Transfer;
            this.Distance = route.Distance;
            LinkedList<List<TransportInfo>> transports = new LinkedList<List<TransportInfo>>();
            route.TransportInfo.ForEach(t =>
            {
                if (!t.IsPassThru)
                    transports.AddLast(new List<TransportInfo>());
                transports.Last.Value.Add(t);
            });
            TransportChunks = transports
                .Select(s => new TransportChunkViewModel(s))
                .Append(new TransportChunkViewModel(new[] { route.TransportInfo.Last() }, true))
                .ToArray();
        }

        public string Name { get; private set; }

        public string TotalTime { get; private set; }

        public string Fare { get; private set; }

        public string TransferCount { get; private set; }

        public string Distance { get; private set; }

        public IEnumerable<TransportChunkViewModel> TransportChunks { get; private set; }
    }

    public class TransportChunkViewModel : ViewModel
    {
        public bool IsLast { get; private set; }
        public TransportChunkViewModel(IEnumerable<TransportInfo> infoc, bool isLast = false)
        {
            this.IsLast = isLast;
            var first = infoc.First();
            if (!isLast)
            {
                this.StationName = first.Origin;
                this.CurrentFare = first.Fare;
                Transports =
                    new[] { new[] { new TransportViewModel(first, true) }, 
                        infoc.Skip(1).Select(s => new TransportViewModel(s)).ToArray() }
                    .SelectMany(_ => _).ToArray();
            }
            else
            {
                this.StationName = first.Destination;
                Transports = new TransportViewModel[0];
            }
        }

        public string CurrentFare { get; set; }

        public string StationName { get; set; }

        public IEnumerable<TransportViewModel> Transports { get; set; }
    }

    public class TransportViewModel : ViewModel
    {
        public TransportInfo TransportInfo { get; private set; }
        public bool IsFirst { get; private set; }
        public TransportViewModel(TransportInfo info, bool isFirst = false)
        {
            this.IsFirst = isFirst;
            this.TransportInfo = info;
        }

        public Color LineColor { get { return TransportInfo.LineColor; } }

        public string LineName { get { return TransportInfo.LineName; } }

        public string Detail { get { return TransportInfo.Detail; } }

        public string Fare { get { return TransportInfo.Fare; } }

        public string Origin { get { return TransportInfo.Origin; } }

        public string DepartureTime { get { return TransportInfo.DepartureTime; } }

        public string ArrivalTime { get { return TransportInfo.ArrivalTime; } }
    }
}
