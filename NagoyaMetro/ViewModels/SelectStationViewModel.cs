using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Hailprism.Spectrum.Linq;
using Hailprism.Spectrum.Mvvm;
using Hailprism.Spectrum.Mvvm.Messaging.Messages;
using Microsoft.Phone.Reactive;
using NagoyaMetro.Models;

namespace NagoyaMetro.ViewModels
{
    public class SelectStationViewModel : ViewModel
    {
        private bool _isOnlyForStation;
        public bool IsOnlyForStation
        {
            get { return _isOnlyForStation; }
        }

        StationInfoReader inforeader;

        public SelectStationViewModel()
        {
            InitializeObservable();
            Setting.StationSearchResult = null;
            this._isOnlyForStation = Setting.StationSearchEditChannel == EditChannel.Via;
            this.inforeader = new StationInfoReader();
            RefreshStationsInfo();
        }

        private bool _isExistsMoreStationsAndBusStops = false;
        public bool IsExistsMoreStationsAndBusStops
        {
            get { return _isExistsMoreStationsAndBusStops; }
            set
            {
                _isExistsMoreStationsAndBusStops = value;
                RaisePropertyChanged(() => IsExistsMoreStationsAndBusStops);
            }
        }

        private bool _isExistsMoreStations = false;
        public bool IsExistsMoreStations
        {
            get { return _isExistsMoreStations; }
            set
            {
                _isExistsMoreStations = value;
                RaisePropertyChanged(() => IsExistsMoreStations);
            }
        }

        private bool _isExistsMoreBusStops = false;
        public bool IsExistsMoreBusStops
        {
            get { return _isExistsMoreBusStops; }
            set
            {
                _isExistsMoreBusStops = value;
                RaisePropertyChanged(() => IsExistsMoreBusStops);
            }
        }

        private string _searchKey = String.Empty;
        public string SearchKey
        {
            get { return _searchKey; }
            set
            {
                _searchKey = value;
                RaisePropertyChanged(() => SearchKey);
                RefreshStationsInfo();
            }
        }

        private bool _isRecentlyUsedStationsShowing = true;
        public bool IsRecentlyUsedStationsShowing
        {
            get { return _isRecentlyUsedStationsShowing; }
            set
            {
                _isRecentlyUsedStationsShowing = value;
                RaisePropertyChanged(() => IsRecentlyUsedStationsShowing);
            }
        }

        private bool _isListConstructing = false;
        public bool IsListConstructing
        {
            get { return _isListConstructing; }
            set
            {
                _isListConstructing = value;
                RaisePropertyChanged(() => IsListConstructing);
            }
        }

        private ObservableCollection<StationViewModel> _stationsAndBusStops =
            new ObservableCollection<StationViewModel>();
        public ObservableCollection<StationViewModel> StationsAndBusStops
        {
            get { return _stationsAndBusStops; }
        }

        private ObservableCollection<StationViewModel> _stations =
            new ObservableCollection<StationViewModel>();
        public ObservableCollection<StationViewModel> Stations
        {
            get { return _stations; }
        }

        private ObservableCollection<StationViewModel> _busStops =
            new ObservableCollection<StationViewModel>();
        public ObservableCollection<StationViewModel> BusStops
        {
            get { return _busStops; }
        }

        Subject<Unit> stationsAndBusStopsScheduler = null;
        Subject<Unit> stationsScheduler = null;
        Subject<Unit> busStopsScheduler = null;
        Subject<StationViewModel> readerSubject = null;
        private void InitializeObservable()
        {
            IsExistsMoreStationsAndBusStops = true;
            IsExistsMoreStations = true;
            IsExistsMoreBusStops = true;
            stationsAndBusStopsScheduler = new Subject<Unit>();
            stationsScheduler = new Subject<Unit>();
            busStopsScheduler = new Subject<Unit>();
            readerSubject = new Subject<StationViewModel>();
            readerSubject
                .BufferWithCount(20)
                .Zip(stationsAndBusStopsScheduler, (l, _) => l)
                .Subscribe(s => s.ForEach(
                    i => StationsAndBusStops.Add(i)),
                    () => IsExistsMoreStationsAndBusStops = false);
            readerSubject
                .Where(v => !v.IsBusStop)
                .BufferWithCount(20)
                .Zip(stationsScheduler, (l, _) => l)
                .Subscribe(s => s.ForEach(
                    i => Stations.Add(i)),
                    () => IsExistsMoreStations = false);
            readerSubject
                .Where(v => v.IsBusStop)
                .BufferWithCount(20)
                .Zip(busStopsScheduler, (l, _) => l)
                .Subscribe(s => s.ForEach(
                    i => BusStops.Add(i)),
                    () => IsExistsMoreBusStops = false);
        }

        private void RefreshStationsInfo()
        {
            // InitializeObservable();
            StationsAndBusStops.Clear();
            Stations.Clear();
            BusStops.Clear();
            IsListConstructing = true;
            InitializeObservable();
            Observable.Start(() => GetStations())
                .SelectMany(_ => _)
                .ObserveOnDispatcher()
                .Subscribe(readerSubject);
            // これは初回の分！！
            ReadNextStationsAndBusStops();
            ReadNextStations();
            ReadNextBusStops();
            IsListConstructing = false;
        }

        #region ReadNextStationsAndBusStopsCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _ReadNextStationsAndBusStopsCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand ReadNextStationsAndBusStopsCommand
        {
            get
            {
                if (_ReadNextStationsAndBusStopsCommand == null)
                {
                    _ReadNextStationsAndBusStopsCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ReadNextStationsAndBusStops);
                }
                return _ReadNextStationsAndBusStopsCommand;
            }
        }

        public void ReadNextStationsAndBusStops()
        {
            if (stationsAndBusStopsScheduler != null)
                stationsAndBusStopsScheduler.OnNext(new Unit());
        }
        #endregion

        #region ReadNextStationsCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _ReadNextStationsCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand ReadNextStationsCommand
        {
            get
            {
                if (_ReadNextStationsCommand == null)
                {
                    _ReadNextStationsCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ReadNextStations);
                }
                return _ReadNextStationsCommand;
            }
        }

        public void ReadNextStations()
        {
            if (stationsScheduler != null)
                stationsScheduler.OnNext(new Unit());
        }
        #endregion

        #region ReadNextBusStopsCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _ReadNextBusStopsCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand ReadNextBusStopsCommand
        {
            get
            {
                if (_ReadNextBusStopsCommand == null)
                {
                    _ReadNextBusStopsCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ReadNextBusStops);
                }
                return _ReadNextBusStopsCommand;
            }
        }

        public void ReadNextBusStops()
        {
            if (busStopsScheduler != null)
                busStopsScheduler.OnNext(new Unit());
        }
        #endregion

        private IEnumerable<StationViewModel> GetStations()
        {
            if (String.IsNullOrEmpty(this.SearchKey))
            {
                // recently used stations
                IsRecentlyUsedStationsShowing = true;
                return Setting.RecentlyUsedStations
                    .Select(p => new StationViewModel(this, p));
            }
            else
            {
                IsRecentlyUsedStationsShowing = false;
                return inforeader.QueryStation(this.SearchKey)
                    .Select(p => new StationViewModel(this, p));
            }
        }

        public void SelectChildren(StationDescription desc)
        {
            Setting.RecentlyUsedStations = new[] { desc }
                .Concat(Setting.RecentlyUsedStations)
                .Distinct().Take(10);
            Setting.StationSearchResult = desc.Name;
            this.RaiseMessageAsync(NavigateMessage.GoBack());
        }
    }

    public class StationViewModel
    {
        SelectStationViewModel _parent;
        StationDescription _sd;
        public StationViewModel(SelectStationViewModel parent, StationDescription sd)
        {
            this._sd = sd;
            this._parent = parent;
        }

        public string Name
        {
            get { return _sd.Name; }
        }

        public string Reading
        {
            get { return _sd.Reading; }
        }

        public bool IsBusStop
        {
            get { return _sd.IsBusStop; }
        }

        #region SelectThisCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _SelectThisCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand SelectThisCommand
        {
            get
            {
                if (_SelectThisCommand == null)
                {
                    _SelectThisCommand = new GalaSoft.MvvmLight.Command.RelayCommand(SelectThis);
                }
                return _SelectThisCommand;
            }
        }

        public void SelectThis()
        {
            _parent.SelectChildren(this._sd);
        }
        #endregion
    }
}