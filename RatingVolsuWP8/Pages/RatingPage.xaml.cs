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
using RatinVolsuAPI;

namespace RatingVolsuWP8
{
    public partial class RatingPage : PhoneApplicationPage
    {
        private readonly RatingViewModel _viewModel;
        public RatingPage()
        {
            InitializeComponent();
            _viewModel = new RatingViewModel();
            DataContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string type;
            string FavItemId;
            RequestManipulation reqManip;
            if (NavigationContext.QueryString.TryGetValue("favoriteItemId", out FavItemId))
            {
                _viewModel.GetFavoriteItem(FavItemId);
                reqManip = _viewModel.CurrentFavoritesItem.GetRequest();
            }
            else
            {
                reqManip = NavigationService.GetNavigationData() as RequestManipulation;
            }
            _viewModel.GetRatingFromDb(reqManip);
            if(NavigationContext.QueryString.TryGetValue("type", out type))
            {
                if (type == RatingType.RatingOfStudent.ToString())
                {
                    SubjectsPanoramaItem.Visibility = Visibility.Collapsed;
                    GroupRatingPanoramaItem.Visibility = Visibility.Collapsed;
                    App.InitProgressIndicator(true, "Загрузка рейтинга студента...", this);
                    if (await App.IsInternetAvailable())
                    {
                        await _viewModel.GetWebRatingOfStudent(reqManip);
                        App.ProgressIndicator.IsVisible = false;
                    }
                    else
                    {
                        App.ProgressIndicator.IsVisible = false;
                        MessageBox.Show("К сожалению, соединение с интернетом недоступно.");
                        return;
                    }
                    
                }
                else
                {
                    App.InitProgressIndicator(true, "Загрузка рейтинга группы...", this);
                    if (await App.IsInternetAvailable())
                    {
                        await _viewModel.GetWebRatingOfGroup(reqManip);
                        App.ProgressIndicator.IsVisible = false;
                    }
                    else
                    {
                        App.ProgressIndicator.IsVisible = false;
                        MessageBox.Show("К сожалению, соединение с интернетом недоступно.");
                        return;
                    }
                }
            }

        }
        #region SelectionChanges

        private void SubjectsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedIndex = SubjectsListBox.SelectedIndex;
            if (selectedIndex == -1) return;
            _viewModel.GetRatingBySubject(selectedIndex);
        }
        private void GroupRatingListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.BallsToNextPlace = 20.ToString();
        }
        #endregion

        
    }
}