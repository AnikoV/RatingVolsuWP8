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
using Rating = RatingVolsuAPI.Rating;

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
                        ApplicationBar.IsVisible = true;
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
                        ApplicationBar.IsVisible = true;
                        
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
        private async void GroupRatingListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var subjLb = sender as ListBox;
            if (subjLb != null)
            {
                var selectedItem = subjLb.SelectedItem as Rating;
                if (selectedItem == null) 
                    return;
                if (!_viewModel.SetStatisticForRating(selectedItem))
                {
                    //TODO не показывать статистику, показать короны)
                }
                
                await _viewModel.GetWebRatingOfStudent(_viewModel.RequestManip);
            }
        }
        #endregion

        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            if (_viewModel.RequestManipForStudent != null)
            {
                ApplicationBar.Buttons.Clear();
                var addFavoritesByGroup =
                    new ApplicationBarIconButton(new Uri("/Assets/Images/AppBar/groupAppBar.png", UriKind.Relative));
                addFavoritesByGroup.Text = "Группа";
                addFavoritesByGroup.Click += ApplicationBarGroupButton_OnClick;
                ApplicationBar.Buttons.Add(addFavoritesByGroup);

                var addFavoritesByStudent =
                    new ApplicationBarIconButton(new Uri("/Assets/Images/AppBar/studentAppBar.png", UriKind.Relative));
                addFavoritesByStudent.Text = "Студент";
                addFavoritesByStudent.Click += ApplicationBarStudentButton_OnClick;
                ApplicationBar.Buttons.Add(addFavoritesByStudent);
            }
            else
            {
                if (!_viewModel.CheckFavorites(false))
                    MessageBox.Show("Запись уже находится в избранном");
                else
                    ShowCustumMessageBox(false);
            }
        }

        private void ShowCustumMessageBox(bool p)
        {
            var textBox = new TextBox()
            {
                Width = 300
            };
            var cmBox = new CustomMessageBox()
            {
                Message = "Введите имя",
                Content = textBox,
                LeftButtonContent = "ok",
                RightButtonContent = "закрыть"

            };
            cmBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        _viewModel.SaveFavorites(p, textBox.Text);
                        NavigationService.Navigate(new Uri("/MainPage.xaml?tofavorites=true", UriKind.Relative));
                        break;
                    default: break;
                }
            };

            cmBox.Show();
        }


        private void ApplicationBarGroupButton_OnClick(object sender, EventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            if (!_viewModel.CheckFavorites(false))
                MessageBox.Show("Запись уже находится в избранном");
            else
                ShowCustumMessageBox(false);
        }

        private void ApplicationBarStudentButton_OnClick(object sender, EventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            if (!_viewModel.CheckFavorites(true))
                MessageBox.Show("Запись уже находится в избранном");
            else
                ShowCustumMessageBox(true);

        }
    }
}