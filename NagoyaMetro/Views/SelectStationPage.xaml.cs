using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace NagoyaMetro.Views
{
    public partial class SelectStationPage : PhoneApplicationPage
    {
        public SelectStationPage()
        {
            InitializeComponent();
        }

        private void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                this.Focus();
        }
    }
}