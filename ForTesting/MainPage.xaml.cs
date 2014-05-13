using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using ForTesting.Resources;
using RatingVolsuAPI;

namespace ForTesting
{
    public partial class MainPage : PhoneApplicationPage
    {
        readonly MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void PivotItem_Tap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/InputDataPage.xaml?type=RatingOfStudent", UriKind.Relative));
        }

        private void PivotItem_Tap_1(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/InputDataPage.xaml?type=RatingOfGroup", UriKind.Relative));
        }

        private void FavoritesList_Tap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = FavoritesList.SelectedIndex;
            if (SelectedIndex == -1) return;
            var id = _viewModel.favoritesCollection[SelectedIndex].Id;
            NavigationService.Navigate(new Uri("/Pages/TestPage.xaml?favoriteitem=" + id, UriKind.Relative));
        }
    }
}