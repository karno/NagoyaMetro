using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using NagoyaMetro.ViewModels;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Reactive;
using GalaSoft.MvvmLight.Threading;

namespace NagoyaMetro.Views
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var vm = this.DataContext as SearchViewModel;
            if (vm != null)
                vm.OnNavigated();

        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            var vm = this.DataContext as SearchViewModel;
            if (vm != null)
            {
                string err = vm.CheckCanFindRoute();
                if (err != null)
                {
                    MessageBox.Show(err);
                }
                else
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                    vm.FindRoute()
                        .Finally(() => DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true))
                        .ObserveOnDispatcher()
                        .Subscribe(
                            _ => { },
                            ex =>
                            {
                                MessageBox.Show("エラーが発生しました。" + Environment.NewLine +
                                    "ネットワークに接続できることを確認してもう一度お試しいただくか、アップデートが無いか検索してください。",
                                    "検索エラー", MessageBoxButton.OK);
                            },
                            () => vm.MoveToResultView());
                }
            }
        }
    }
}