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
using Microsoft.Phone.Tasks;
using NagoyaMetro.ViewModels;
using NagoyaMetro.Models;
using Hailstone.Linq;

namespace NagoyaMetro.Views
{
    public partial class ResultPage : PhoneApplicationPage
    {
        public ResultPage()
        {
            InitializeComponent();
        }

        private ResultViewModel ViewModel
        {
            get { return this.DataContext as ResultViewModel; }
        }


        private void TweetButton_Click(object sender, EventArgs e)
        {
            var task = new ShareStatusTask();
            var status = new[]{Setting.LastSearchDescription.Origin, Setting.LastSearchDescription.Via, Setting.LastSearchDescription.Destination}
                .Where(s => !String.IsNullOrEmpty(s))
                .JoinString(" -> ");
            task.Status = status + " #NagoyaMetro";
            task.Show();
        }
    }
}