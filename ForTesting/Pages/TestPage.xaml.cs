using System;
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
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.TryGetValue("favoriteitem", out ItemId))
            {
                _viewModel.GetFavoriteItem(ItemId);
                _viewModel.GetRatingFromDb();
            }
            else
            {
                string facultId, groupId, semestr, studentId;
                NavigationContext.QueryString.TryGetValue("facult", out facultId);
                NavigationContext.QueryString.TryGetValue("group", out groupId);
                NavigationContext.QueryString.TryGetValue("semestr", out semestr);
                NavigationContext.QueryString.TryGetValue("student", out studentId);
                _viewModel.CreateRequest(facultId, groupId, semestr, studentId);
                await _viewModel.GetRatingFromServer();

            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _viewModel.CurrentFavoritesItem = null;
        }

        private void List_Tap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = List.SelectedIndex;
            if (SelectedIndex == -1) return;
            _viewModel.GetFavoritesRating();
        }

        private async void StudentListTap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = StudentList.SelectedIndex;
            if (SelectedIndex == -1) return;
            await _viewModel.GetRatingOfStudent(SelectedIndex);
        }
    }
}