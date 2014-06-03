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
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace ForTesting
{

    public partial class TestPage : PhoneApplicationPage
    {
        private RequestDataViewModel _viewModel;

        public TestPage()
        {
            InitializeComponent();
            InitializeAppBar();
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
            {
                App.InitProgressIndicator(true, "Загрузка рейтинга студента...", this);
                await _viewModel.GetRatingOfStudentFromServer(req);
                App.ProgressIndicator.IsVisible = false;
            }
            else
            {
                App.InitProgressIndicator(true, "Загрузка рейтинга группы...", this);
                await _viewModel.GetRatingOfGroupFromServer(req);
                App.ProgressIndicator.IsVisible = false;
            }
                

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _viewModel.CurrentFavoritesItem = null;
        }

        private async void StudentListTap(object sender, GestureEventArgs e)
        {
            var selectedIndex = StudentList.SelectedIndex;
            if (selectedIndex == -1) return;
            await _viewModel.GetRatingOfStudent(selectedIndex);
        }

        private void SubjectList_Tap(object sender, GestureEventArgs e)
        {
            var selectedIndex = SubjectList.SelectedIndex;
            if (selectedIndex == -1) return;
            _viewModel.GetRatingBySubject(selectedIndex);
        }

        #region AppBar

        private void InitializeAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            var addFavoritesByGroup =
                new ApplicationBarIconButton(new Uri("/Images/Tiles/check.png", UriKind.Relative));
            addFavoritesByGroup.Text = "В Избранное";
            addFavoritesByGroup.Click += appBarButtonGroup_Click;
            ApplicationBar.Buttons.Add(addFavoritesByGroup);

        }

        private void appBarButtonGroup_Click(object sender, EventArgs e)
        {
            if (!_viewModel.SaveFavorites())
                MessageBox.Show("Низя!");
        }

        #endregion
    }
}