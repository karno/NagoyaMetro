using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using Hailprism.Spectrum.Linq;
using Hailprism.Spectrum.Mvvm;
using NagoyaMetro.Models.Routing;

namespace NagoyaMetro.Models
{
    public static class Setting
    {
        #region Declaration and utilities

        static IsolatedStorageSettings _setting = IsolatedStorageSettings.ApplicationSettings;

        public static T GetValueOrDefault<T>(this IsolatedStorageSettings settings, string key, T defaultValue)
        {
            if (ViewModel.IsInDesignModeStatic)
            {
                return defaultValue;
            }
            else
            {
                T value;
                if (settings.TryGetValue(key, out value))
                    return value;
                else
                    return defaultValue;
            }
        }

        #endregion

        /// <summary>
        /// マイルート
        /// </summary>
        public static IEnumerable<RouteDescription> MyRoutes
        {
            get { return _setting.GetValueOrDefault("MyRoutes", new List<RouteDescription>()); }
            set
            {
                _setting["MyRoutes"] = value.Guard().ToList();
                _setting.Save();
            }
        }

        /// <summary>
        /// 最近使った駅
        /// </summary>
        public static IEnumerable<StationDescription> RecentlyUsedStations
        {
            get { return _setting.GetValueOrDefault("RecentlyUsedStations", new List<StationDescription>()); }
            set
            {
                _setting["RecentlyUsedStations"] = value.Guard().ToList();
                _setting.Save();
            }
        }

        /// <summary>
        /// 最後に選択した優先条件
        /// </summary>
        public static PreferKey LastPreferKey
        {
            get { return _setting.GetValueOrDefault("LastPreferKey", PreferKey.None); }
            set
            {
                _setting["LastPreferKey"] = value;
                _setting.Save();
            }
        }

        /// <summary>
        /// 現在の検索条件ホルダー
        /// </summary>
        public static RouteDescription CurrentSearchConstraint
        {
            get { return _setting.GetValueOrDefault("CurrentSearchConstraint", new RouteDescription()).FreezeClone(); }
            set
            {
                _setting["CurrentSearchConstraint"] = value;
                _setting.Save();
            }
        }

        public static EditChannel StationSearchEditChannel
        {
            get { return _setting.GetValueOrDefault("EditChannel", EditChannel.None); }
            set
            {
                _setting["EditChannel"] = value;
                _setting.Save();
            }
        }

        /// <summary>
        /// 駅名検索結果ホルダー
        /// </summary>
        public static string StationSearchResult
        {
            get { return _setting.GetValueOrDefault("StationSearchResult", (string)null); }
            set
            {
                _setting["StationSearchResult"] = value;
                _setting.Save();
            }
        }

        /// <summary>
        /// 最後の検索条件
        /// </summary>
        public static SearchDescription LastSearchDescription
        {
            get { return _setting.GetValueOrDefault("LastSearchDescription", new SearchDescription()); }
            set
            {
                _setting["LastSearchDescription"] = value;
                _setting.Save();
            }
        }

        /// <summary>
        /// 最後の検索結果
        /// </summary>
        public static IEnumerable<TransportRoute> LastResult
        {
            get{return _setting.GetValueOrDefault("LastResult", new List<TransportRoute>());}
            set
            {
                _setting["LastResult"] = value.Guard().ToList();
                _setting.Save();
            }
        }

        const int MyRouteIncrementValue = 3;
        const int MyRouteMaxValue = 20;

        public static void UpdateMyRoutes(RouteDescription desc)
        {
            desc = desc.FreezeClone();
            IEnumerable<RouteDescription> routes = MyRoutes.ToArray();
            var pd = routes.Where(rd => rd.Equals(desc)).FirstOrDefault();
            if (pd != null)
            {
                pd.RouteCount += MyRouteIncrementValue + 1; // it will decrement later.
                if (pd.RouteCount > MyRouteMaxValue) // assert largest boundary
                    pd.RouteCount = MyRouteMaxValue;
            }
            else
            {
                desc.RouteCount = MyRouteIncrementValue + 1;
                routes = routes.Append(desc);
            }
            // decrement value
            routes.ForEach(r => r.RouteCount--);
            // re-ordering
            routes = routes
                .Where(o => o.RouteCount > 0)
                .OrderByDescending(o => o.RouteCount);
            // set result
            MyRoutes = routes;
        }
    }

    public enum EditChannel
    {
        None,
        Origin,
        Destination,
        Via,
    }
}
