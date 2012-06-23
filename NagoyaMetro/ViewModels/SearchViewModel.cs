using System;
using Hailstone.Linq;
using Hailstone.Mvvm;
using Hailstone.Mvvm.Messaging.Messages;
using Microsoft.Phone.Reactive;
using NagoyaMetro.Models;
using NagoyaMetro.Models.Routing;

namespace NagoyaMetro.ViewModels
{
    public class SearchViewModel : ViewModel
    {
        RouteDescription _route;
        public SearchViewModel()
        {
            if (this.IsInDesignMode)
            {
                _route = new RouteDescription();
                _preferType = 0;
            }
            else
            {
                _route = Setting.CurrentSearchConstraint;
                if (_route == null)
                    _route = new RouteDescription();
                _preferType = (int)Setting.LastPreferKey;
            }
        }

        public string SearchTitle
        {
            get
            {
                if (_route.IsSearchOnlyBus)
                    return "バスの検索";
                else
                    return "路線の検索";
            }
        }

        private EditChannel EditChannel
        {
            get { return Setting.StationSearchEditChannel; }
            set { Setting.StationSearchEditChannel = value; }
        }

        private void NavigateToEdit()
        {
            // impl
            Setting.StationSearchResult = null;
            this.RaiseMessageAsync(NavigateMessage.Navigate(
                new Uri("/Views/SelectStationPage.xaml", UriKind.Relative)));
        }

        #region OnNavigatedCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _OnNavigatedCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand OnNavigatedCommand
        {
            get
            {
                if (_OnNavigatedCommand == null)
                {
                    _OnNavigatedCommand = new GalaSoft.MvvmLight.Command.RelayCommand(OnNavigated);
                }
                return _OnNavigatedCommand;
            }
        }

        public void OnNavigated()
        {
            switch (EditChannel)
            {
                case EditChannel.Origin:
                    this.Origin = Setting.StationSearchResult;
                    break;
                case EditChannel.Destination:
                    this.Destination = Setting.StationSearchResult;
                    break;
                case EditChannel.Via:
                    this.Via = Setting.StationSearchResult;
                    break;
            }
            EditChannel = EditChannel.None;
        }
        #endregion

        public string Origin
        {
            get { return _route.Origin; }
            set
            {
                _route.Origin = value;
                RaisePropertyChanged(() => Origin);
            }
        }

        #region EditOriginCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _EditOriginCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand EditOriginCommand
        {
            get
            {
                if (_EditOriginCommand == null)
                {
                    _EditOriginCommand = new GalaSoft.MvvmLight.Command.RelayCommand(EditOrigin);
                }
                return _EditOriginCommand;
            }
        }

        public void EditOrigin()
        {
            EditChannel = EditChannel.Origin;
            NavigateToEdit();
        }
        #endregion

        public string Destination
        {
            get { return _route.Destination; }
            set
            {
                _route.Destination = value;
                RaisePropertyChanged(() => Destination);
            }
        }

        #region EditDestinationCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _EditDestinationCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand EditDestinationCommand
        {
            get
            {
                if (_EditDestinationCommand == null)
                {
                    _EditDestinationCommand = new GalaSoft.MvvmLight.Command.RelayCommand(EditDestination);
                }
                return _EditDestinationCommand;
            }
        }

        public void EditDestination()
        {
            EditChannel = EditChannel.Destination;
            NavigateToEdit();
        }
        #endregion

        public string Via
        {
            get { return _route.Via; }
            set
            {
                _route.Via = value;
                RaisePropertyChanged(() => Via);
            }
        }

        #region EditViaCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _EditViaCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand EditViaCommand
        {
            get
            {
                if (_EditViaCommand == null)
                {
                    _EditViaCommand = new GalaSoft.MvvmLight.Command.RelayCommand(EditVia);
                }
                return _EditViaCommand;
            }
        }

        public void EditVia()
        {
            EditChannel = EditChannel.Via;
            NavigateToEdit();
        }
        #endregion

        public bool IsBusSearchMode { get { return _route.IsSearchOnlyBus; } }

        private DateTime _time = DateTime.Now;
        public DateTime Time
        {
            get { return _time; }
            set
            {
                _time = value;
                System.Diagnostics.Debug.WriteLine(value);
                RaisePropertyChanged(() => Time);
            }
        }

        public bool IsTimeEnabled
        {
            get { return _timeType < 2; }
        }

        private int _timeType = 0;
        public int TimeType
        {
            get { return _timeType; }
            set
            {
                _timeType = value;
                RaisePropertyChanged(() => TimeType);
                RaisePropertyChanged(() => IsTimeEnabled);
            }
        }

        private int _preferType = 0;
        public int PreferType
        {
            get { return _preferType; }
            set
            {
                _preferType = value;
                RaisePropertyChanged(() => PreferType);
            }
        }

        private bool _isFindingRoute = false;
        public bool IsFindingRoute
        {
            get { return _isFindingRoute; }
            set
            {
                _isFindingRoute = value;
                RaisePropertyChanged(() => IsFindingRoute);
            }
        }

        /// <summary>
        /// ルートを探せる状態であるかを確認します。
        /// </summary>
        /// <returns>エラーメッセージが返ります、nullならOK</returns>
        public string CheckCanFindRoute()
        {
            if (String.IsNullOrEmpty(Origin))
                return "出発地が設定されていません。";
            if (String.IsNullOrEmpty(Destination))
                return "目的地が設定されていません。";
            if (Origin == Destination)
                return "出発地と目的地が同一です。";
            if (Origin == Via)
                return "出発地と経由地が同一です。";
            if (Destination == Via)
                return "目的地と経由地が同一です。";
            return null;
        }

        public IObservable<Unit> FindRoute()
        {
            if (CheckCanFindRoute() != null || IsFindingRoute)
                throw new InvalidOperationException();
            IsFindingRoute = true;
            // build SeachDescription
            SearchDescription sd = new SearchDescription(
                this._route,
                (PreferKey)_preferType,
                _time,
                (TimeType)_timeType);
            Setting.LastSearchDescription = sd;
            Setting.LastPreferKey = sd.PreferKey;
            Setting.UpdateMyRoutes(this._route);
            Setting.LastResult = null;
            return TransferNaviHandler.QueryTransportInfo(sd)
                .Do(result => Setting.LastResult = Setting.LastResult.Append(result))
                .Finally(() => IsFindingRoute = false)
                .Select(_ => new Unit());
        }

        public void MoveToResultView()
        {
            this.RaiseMessageAsync(
                NavigateMessage.Navigate(new Uri("/Views/ResultPage.xaml", UriKind.Relative)));
        }
    }
}