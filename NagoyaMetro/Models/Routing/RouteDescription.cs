using Hailstone.Linq;

namespace NagoyaMetro.Models.Routing
{
    /// <summary>
    /// 時刻を除く検索条件を定義します。
    /// </summary>
    public class RouteDescription
    {
        /// <summary>
        /// 出発地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// 経由地
        /// </summary>
        public string Via { get; set; }

        /// <summary>
        /// バスのみで探す
        /// </summary>
        public bool IsSearchOnlyBus { get; set; }

        public override bool Equals(object obj)
        {
            var rd = obj as RouteDescription;
            return rd != null &&
                rd.Origin == this.Origin &&
                rd.Destination == this.Destination &&
                rd.Via == this.Via &&
                rd.IsSearchOnlyBus == this.IsSearchOnlyBus;
        }

        public override int GetHashCode()
        {
            return new[] { this.Origin, this.Destination, this.Via, this.IsSearchOnlyBus.ToString() }
                .JoinString("\t").GetHashCode();
        }

        public RouteDescription FreezeClone()
        {
            return new RouteDescription()
            {
                Origin = this.Origin,
                Destination = this.Destination,
                Via = this.Via,
                IsSearchOnlyBus = this.IsSearchOnlyBus,
            };
        }
    }
}
