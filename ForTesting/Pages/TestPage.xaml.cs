﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RatingVolsuAPI;
using RatinVolsuAPI;

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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            string ItemId;
            string type;
            RequestManipulation req;
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.TryGetValue("favoriteitem", out ItemId))
            {
                _viewModel.GetFavoriteItem(ItemId);
                req = _viewModel.CurrentFavoritesItem.GetRequest();
            }
            else
            {
                req = (RequestManipulation) NavigationService.GetNavigationData();
            }
            NavigationContext.QueryString.TryGetValue("type", out type);
            _viewModel.GetRatingFromDb(req);
            if (type == RatingType.RatingOfStudent.ToString())
                await _viewModel.GetRatingOfStudentFromServer(req);
            else
                await _viewModel.GetRatingOfGroupFromServer(req);

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _viewModel.CurrentFavoritesItem = null;
        }

        private async void StudentListTap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = StudentList.SelectedIndex;
            if (SelectedIndex == -1) return;
            await _viewModel.GetRatingOfStudent(SelectedIndex);
        }
    }
}