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
                    await _viewModel.GetWebRatingOfStudent(reqManip);
                }
                else
                {
                    await _viewModel.GetWebRatingOfGroup(reqManip);
                }
            }

        }
        #region SelectionChanges

        private void SubjectsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}