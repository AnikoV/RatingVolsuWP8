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

namespace ForTesting
{
    public partial class TestPage : PhoneApplicationPage
    {
        private RequestDataViewModel _viewModel;

        public TestPage()
        {
            InitializeComponent();

            _viewModel = new RequestDataViewModel();
            DataContext = _viewModel;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
           
            if (RequestInfo.CurrentRatingType == RatingType.RatingOfStudent)
                _viewModel.GetRatingOfStudent();
            else
                _viewModel.GetRatingOfGroup();
        }

        private void List_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var SelectedIndex = List.SelectedIndex;
            if (SelectedIndex == -1) return;
            _viewModel.GetFavoritesRating();
        }
        
    }
}