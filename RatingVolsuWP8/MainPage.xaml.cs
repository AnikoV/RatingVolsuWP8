using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RatingVolsuAPI;
using System.Threading.Tasks;

namespace RatingVolsuWP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.CacheManager.CurrentRatingType = RatingType.RatingOfStudent;
            NavigationService.Navigate(new Uri("/Pages/InputData.xaml", UriKind.Relative));
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            App.CacheManager.CurrentRatingType = RatingType.RatingOfGroup;
            NavigationService.Navigate(new Uri("/Pages/InputData.xaml", UriKind.Relative));
        }

        private  void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            //RequestManager rm = new RequestManager();
            //rm.GetRatingCurrentYear();
        }

        
    }
}