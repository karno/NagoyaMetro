using System;
using System.Text;

namespace NagoyaMetro.Models.Routing
{
    /// <summary>
    /// 検索クエリを示します。<para />
    /// ルート情報に時刻情報を追加したものです。
    /// </summary>
    public class SearchDescription
    {
        private RouteDescription _route;

        public SearchDescription(RouteDescription routing, PreferKey preferKey, DateTime time, TimeType type)
        {
            this._route = routing;
            this.PreferKey = preferKey;
            this.Time = time;
            this.TimeType = type;
        }

        public SearchDescription()
        {
            this._route = new RouteDescription();
        }

        /// <summary>
        /// 出発地
        /// </summary>
        public string Origin
        {
            get { return _route.Origin; }
            set { _route.Origin = value; }
        }

        /// <summary>
        /// 目的地
        /// </summary>
        public string Destination
        {
            get { return _route.Destination; }
            set { _route.Destination = value; }
        }

        /// <summary>
        /// 経由地
        /// </summary>
        public string Via
        {
            get { return _route.Via; }
            set { _route.Via = value; }
        }

        /// <summary>
        /// バスのみで探す
        /// </summary>
        public bool IsSearchOnlyBus
        {
            get { return _route.IsSearchOnlyBus; }
            set { _route.IsSearchOnlyBus = value; }
        }

        /// <summary>
        /// 検索オプション値
        /// </summary>
        public PreferKey PreferKey { get; set; }

        /// <summary>
        /// 出発/到着時間
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 検索時刻種別
        /// </summary>
        public TimeType TimeType { get; set; }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append(TrimAfterSlash(this.Origin));
            ret.Append("→");
            if (!String.IsNullOrEmpty(this.Via))
            {
                ret.Append(this.Via);
                ret.Append("→");
            }
            ret.Append(TrimAfterSlash(this.Destination));
            ret.Append(" ");
            switch (this.TimeType)
            {
                case Routing.TimeType.DepartureTime:
                    ret.Append(Time.ToString("MM/dd HH:mm "));
                    ret.Append("発");
                    break;
                case Routing.TimeType.ArrivalTime:
                    ret.Append(Time.ToString("MM/dd HH:mm "));
                    ret.Append("着");
                    break;
                case Routing.TimeType.FirstTrain:
                    ret.Append(Time.ToString("MM/dd "));
                    ret.Append("始発");
                    break;
                case Routing.TimeType.LastTrain:
                    ret.Append(Time.ToString("MM/dd "));
                    ret.Append("終電");
                    break;
            }
            switch (this.PreferKey)
            {
                case Routing.PreferKey.Cheaper:
                    ret.Append("[安く]");
                    break;
                case Routing.PreferKey.LesserTransfer:
                    ret.Append("[乗換少なく]");
                    break;
            }
            return ret.ToString();
        }

        public string TrimAfterSlash(string raw)
        {
            int idx = -1;
            if ((idx = raw.IndexOf("／")) >= 0)
                return raw.Substring(0, idx);
            else
                return raw;
        }
    }

    /// <summary>
    /// 検索オプション
    /// </summary>
    public enum PreferKey
    {
        /// <summary>
        /// 条件なし
        /// </summary>
        None,
        /// <summary>
        /// 乗換回数少なく
        /// </summary>
        LesserTransfer,
        /// <summary>
        /// 安く
        /// </summary>
        Cheaper
    }

    /// <summary>
    /// 時間の検索種別
    /// </summary>
    public enum TimeType
    {
        /// <summary>
        /// 出発時間
        /// </summary>
        DepartureTime,
        /// <summary>
        /// 到着時間
        /// </summary>
        ArrivalTime,
        /// <summary>
        /// 始発 (午前4:00以降の出発)
        /// </summary>
        FirstTrain,
        /// <summary>
        /// 終電 (午前4:00までの到着)
        /// </summary>
        LastTrain,
    }

}
