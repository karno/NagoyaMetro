using System.Windows.Media;

namespace NagoyaMetro.Models.Routing
{
    /// <summary>
    /// ルートの探索結果
    /// </summary>
    public class TransportRoute
    {
        /// <summary>
        /// ルート名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 時間
        /// </summary>
        public string TotalTime { get; set; }

        /// <summary>
        /// 運賃
        /// </summary>
        public string Fare { get; set; }

        /// <summary>
        /// 乗り換え
        /// </summary>
        public string Transfer { get; set; }

        /// <summary>
        /// 距離
        /// </summary>
        public string Distance { get; set; }

        /// <summary>
        /// 交通手段の情報
        /// </summary>
        public TransportInfo[] TransportInfo { get; set; }
    }

    public class TransportInfo
    {
        public TransportInfo() { }

        /// <summary>
        /// 路線カラー
        /// </summary>
        public Color LineColor { get; set; }

        /// <summary>
        /// 路線名
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// 路線詳細（乗る電車・バスの詳細）
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 運賃
        /// </summary>
        public string Fare { get; set; }

        /// <summary>
        /// 出発地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 到着地
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// 出発時刻
        /// </summary>
        public string DepartureTime { get; set; }

        /// <summary>
        /// 到着時刻
        /// </summary>
        public string ArrivalTime { get; set; }

        /// <summary>
        /// 直前の路線から引き継ぎ乗車(運賃を一体計算の対象にする)
        /// </summary>
        public bool IsPassThru { get; set; }
    }
}
