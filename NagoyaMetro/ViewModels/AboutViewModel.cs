using System;
using System.Reflection;
using Hailstone.Mvvm;
using Microsoft.Phone.Tasks;

namespace NagoyaMetro.ViewModels
{
    public class AboutViewModel : ViewModel
    {
        public string Version
        {
            get
            {
                var name = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
                var version = name.Version;
                return version.Major + "." + version.Minor;
            }
        }

        #region ShowOfficialWebCommand
        private GalaSoft.MvvmLight.Command.RelayCommand _ShowOfficialWebCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand ShowOfficialWebCommand
        {
            get
            {
                if (_ShowOfficialWebCommand == null)
                {
                    _ShowOfficialWebCommand = new GalaSoft.MvvmLight.Command.RelayCommand(ShowOfficialWeb);
                }
                return _ShowOfficialWebCommand;
            }
        }

        public void ShowOfficialWeb()
        {
            var webtask = new WebBrowserTask();
            webtask.Uri = new Uri("http://kotsuk.city.nagoya.jp/route/web/exp.cgi");
            webtask.Show();
        }
        #endregion
    }
}
