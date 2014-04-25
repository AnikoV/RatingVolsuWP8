using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RatingVolsuAPI;

namespace RatingVolsuWP8
{
    public partial class TestPage : PhoneApplicationPage
    {
        private TestViewModel _viewModel;
        public TestPage()
        {
            InitializeComponent();
           // InitializeAppBar();
            _viewModel = new TestViewModel();
            DataContext = _viewModel;
            _viewModel.facultCollection.Add(new Facult() {Name = "dsfs"});

            
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);
        //    string st;
        //    NavigationContext.QueryString.TryGetValue("facult", out st);
        //    _viewModel.FacultId = st;

        //}


        //private void InitializeAppBar()
        //{
        //    ApplicationBar = new ApplicationBar();
        //    ApplicationBar.IsVisible = true;
        //    ApplicationBar.IsMenuEnabled = true;

        //    var appBarButtonOK =
        //        new ApplicationBarIconButton(new Uri("/Images/AppBar/check.png", UriKind.Relative));
        //    appBarButtonOK.Text = "ОК";
        //    appBarButtonOK.Click += appBarButtonOk_Click;
        //    ApplicationBar.Buttons.Add(appBarButtonOK);
        //}

        //private void appBarButtonOk_Click(object sender, EventArgs e)
        //{
        //    Manager.gr = new ObservableCollection<Group>(App.ViewModel.groupCollection);
        //    _viewModel.SaveChangesRatingtoDb(App.CurrentFavorites);

        //}
    }
}