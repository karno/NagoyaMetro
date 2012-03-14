using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Hailprism.Spectrum.Linq;
using Hailprism.Spectrum.Mvvm;
using Hailprism.Spectrum.Mvvm.Messaging.Messages;
using Microsoft.Phone.Reactive;
using NagoyaMetro.Models;
using NagoyaMetro.Models.Routing;

namespace NagoyaMetro.ViewModels
{
    public class MainViewModel : ViewModel
    {
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

        public IEnumerable<MyRouteViewModel> MyRoutes
        {
            get
            {
                return Setting.MyRoutes
                    .Take(8)
                    .Select(r => new MyRouteViewModel(this, r));
            }
        }

        public void RefreshMyRoutes()
        {
            RaisePropertyChanged(() => MyRoutes);
        }

        #region SearchTrainCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _SearchTrainCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand SearchTrainCommand
        {
            get
            {
                if (_SearchTrainCommand == null)
                {
                    _SearchTrainCommand = new GalaSoft.MvvmLight.Command.RelayCommand(SearchTrain);
                }
                return _SearchTrainCommand;
            }
        }

        public void SearchTrain()
        {
            Setting.CurrentSearchConstraint = new RouteDescription() { IsSearchOnlyBus = false };
            this.GoSearch();
        }
        #endregion

        #region SearchBusOnlyCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _SearchBusOnlyCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand SearchBusOnlyCommand
        {
            get
            {
                if (_SearchBusOnlyCommand == null)
                {
                    _SearchBusOnlyCommand = new GalaSoft.MvvmLight.Command.RelayCommand(SearchBusOnly);
                }
                return _SearchBusOnlyCommand;
            }
        }

        public void SearchBusOnly()
        {
            Setting.CurrentSearchConstraint = new RouteDescription() { IsSearchOnlyBus = true };
            this.GoSearch();
        }
        #endregion

        public void GoSearch()
        {
            this.RaiseMessageAsync(
                NavigateMessage.Navigate(new Uri("/Views/SearchPage.xaml",
                    UriKind.Relative)));
        }

        public void GoSearchRoute(RouteDescription route)
        {
            if (IsFindingRoute)
                return;
            IsFindingRoute = true;
            SearchDescription sd = new SearchDescription(
                route,
                Setting.LastPreferKey,
                DateTime.Now,
                 TimeType.DepartureTime);
            Setting.LastSearchDescription = sd;
            Setting.UpdateMyRoutes(route);
            Setting.LastResult = null;
            TransferNaviHandler.QueryTransportInfo(sd)
                .Finally(() => IsFindingRoute = false)
                .Subscribe(result => Setting.LastResult = Setting.LastResult.Append(result),
                    ex =>
                    {
                        this.RaiseMessageAsync(new MessageBoxMessage(
                            "エラーが発生しました。" + Environment.NewLine +
                            "ネットワークに接続できることを確認してもう一度お試しいただくか、アップデートが無いか検索してください。",
                            "検索エラー", MessageBoxButton.OK));
                    },
                    () => this.RaiseMessageAsync(
                    NavigateMessage.Navigate(new Uri("/Views/ResultPage.xaml", UriKind.Relative))));
        }
    }

    public class MyRouteViewModel : ViewModel
    {
        private MainViewModel _parent;

        public RouteDescription Route { get; private set; }

        public MyRouteViewModel(MainViewModel parent, RouteDescription routedesc)
        {
            this._parent = parent;
            this.Route = routedesc;
        }

        public string RouteName
        {
            get
            {
                var descs = (IEnumerable<string>)(new[] { this.Route.Origin });
                if (!string.IsNullOrEmpty(this.Route.Via))
                    descs = descs.Append(this.Route.Via);
                descs = descs.Append(this.Route.Destination);
                return descs.JoinString(Environment.NewLine + "→");
            }
        }

        public string SearchType
        {
            get
            {
                if (this.Route.IsSearchOnlyBus)
                    return "バスのみ検索";
                else
                    return String.Empty;
            }
        }

        #region SelectRouteCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _SelectRouteCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand SelectRouteCommand
        {
            get
            {
                if (_SelectRouteCommand == null)
                {
                    _SelectRouteCommand = new GalaSoft.MvvmLight.Command.RelayCommand(SelectRoute);
                }
                return _SelectRouteCommand;
            }
        }

        public void SelectRoute()
        {
            _parent.GoSearchRoute(this.Route);
        }
        #endregion

        #region SelectRouteByTimeCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _SelectRouteByTimeCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand SelectRouteByTimeCommand
        {
            get
            {
                if (_SelectRouteByTimeCommand == null)
                {
                    _SelectRouteByTimeCommand = new GalaSoft.MvvmLight.Command.RelayCommand(SelectRouteByTime);
                }
                return _SelectRouteByTimeCommand;
            }
        }

        public void SelectRouteByTime()
        {
            Setting.CurrentSearchConstraint = this.Route;//.FreezeClone();
            _parent.GoSearch();
        }
        #endregion

        #region RemoveRouteCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _RemoveRouteCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand RemoveRouteCommand
        {
            get
            {
                if (_RemoveRouteCommand == null)
                {
                    _RemoveRouteCommand = new GalaSoft.MvvmLight.Command.RelayCommand(RemoveRoute);
                }
                return _RemoveRouteCommand;
            }
        }

        public void RemoveRoute()
        {
            Setting.MyRoutes = Setting.MyRoutes
                .Where(r => r.Origin != this.Route.Origin ||
                    r.Via != this.Route.Via ||
                    r.Destination != this.Route.Destination ||
                    r.IsSearchOnlyBus != this.Route.IsSearchOnlyBus).ToArray();
            _parent.RefreshMyRoutes();
        }
        #endregion

    }
}