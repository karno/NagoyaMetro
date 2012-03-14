using Microsoft.Phone.Controls;
using NagoyaMetro.ViewModels;

namespace NagoyaMetro.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var vm = this.DataContext as MainViewModel;
            if (vm != null)
                vm.RefreshMyRoutes();
        }
    }
}