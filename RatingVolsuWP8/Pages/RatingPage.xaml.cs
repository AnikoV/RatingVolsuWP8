using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            InitAppBar();
            _viewModel = new RatingViewModel();
            DataContext = _viewModel;
        }

        private void InitAppBar()
        {
            ApplicationBar.Buttons.Clear();
            var addFavoritesButton = new ApplicationBarIconButton()
            {
                IconUri = new Uri("/Assets/Images/AppBar/favs.addto.png", UriKind.Relative),
                Text = "избранное"
            };
            addFavoritesButton.Click += ApplicationBarIconButton_OnClick;
            ApplicationBar.Buttons.Add(addFavoritesButton);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            var page = App.RootFrame.Content as Page;
            if (page != null)
            {
                var service = page.NavigationService;
                var count = service.BackStack.Count() - 1;
                for (var i = 0; i < count; i++)
                {
                    service.RemoveBackEntry();
                }
                if (service.CanGoBack)
                    service.GoBack();
            }
           // NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string type, tmp;

            var reqManip = NavigationService.GetNavigationData() as RequestManipulation;
            if (reqManip == null) return;
            _viewModel.SetCurrentRequest(reqManip);
            _viewModel.GetRatingFromDb(reqManip);

            if (reqManip.GetType() == typeof(RequestByStudent))
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
                StudentPanoramaItem.Visibility = Visibility.Collapsed;
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
                _viewModel.SetStatisticForRating(selectedItem);
                _viewModel.RequestManipForStudent = new RequestByStudent()
                {
                    FacultId = _viewModel.RequestManip.FacultId,
                    GroupId = _viewModel.RequestManip.GroupId,
                    Semestr = _viewModel.RequestManip.Semestr,
                    StudentId = selectedItem.Student.Id
                };
                if (StudentPanoramaItem.Visibility == Visibility.Collapsed)
                    StudentPanoramaItem.Visibility = Visibility.Visible;
                App.InitProgressIndicator(true, "Загрузка рейтинга студента...", this);
                if (await App.IsInternetAvailable())
                {
                    await _viewModel.GetWebRatingOfStudent(_viewModel.RequestManipForStudent);
                    App.ProgressIndicator.IsVisible = false;
                    SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
                }
                else
                {
                    App.ProgressIndicator.IsVisible = false;
                    MessageBox.Show("К сожалению, соединение с интернетом недоступно.");
                    return;
                }
               
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
                    ShowCustomMessageBox(false);
            }
            
        }

        private void ShowCustomMessageBox(bool p)
        {
            var textBox = new TextBox()
            {
                Width = 300,
                Text = "Без названия",
                HorizontalAlignment = HorizontalAlignment.Left
            };
            textBox.GotFocus += (sender, args) => ((TextBox)sender).SelectAll();            
            var cmBox = new CustomMessageBox()
            {
                Message = "Введите название",
                Content = textBox,
                LeftButtonContent = "Оk",
                RightButtonContent = "Закрыть"

            };
            cmBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        _viewModel.SaveFavorites(p, textBox.Text);
                        Info.ToFavoritesPivot = true;
                        var page = App.RootFrame.Content as Page;
                        if (page != null)
                        {
                            var service = page.NavigationService;
                            int count = service.BackStack.Count() - 1;
                            for (int i = 0; i < count; i++)
                            {
                                service.RemoveBackEntry();
                            }
                            if (service.CanGoBack)
                                service.GoBack();
                        }
                        break;

                        default: break;
                }
                InitAppBar();
            };
            cmBox.Show();
        }

        private void ApplicationBarGroupButton_OnClick(object sender, EventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            if (!_viewModel.CheckFavorites(false))
            {
                MessageBox.Show("Запись уже находится в избранном");
                InitAppBar();
            }
            else
                ShowCustomMessageBox(false);
        }

        private void ApplicationBarStudentButton_OnClick(object sender, EventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            if (!_viewModel.CheckFavorites(true))
            {
                MessageBox.Show("Запись уже находится в избранном");
                InitAppBar();
            }
            else
                ShowCustomMessageBox(true);
        }

        private void RatingPage_OnOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            Debug.WriteLine("Orientation has been changed");
            if (_viewModel.RequestManipForStudent != null)
            {
                if (e.Orientation == PageOrientation.LandscapeRight || e.Orientation == PageOrientation.LandscapeLeft)
                {
                    VerticalState.Visibility = Visibility.Collapsed;
                    HorizontalState.Visibility = Visibility.Visible;
                    SystemTray.IsVisible = false;
                }
                else
                {
                    VerticalState.Visibility = Visibility.Visible;
                    HorizontalState.Visibility = Visibility.Collapsed;
                    SystemTray.IsVisible = true;
                }
            }
        }
    }
}