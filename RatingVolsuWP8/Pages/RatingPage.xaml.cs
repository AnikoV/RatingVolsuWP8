using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        private bool isSelectorEnabled = false;
        private bool _requestExecution;
        private int _selectedStudent = -1;
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
            isSelectorEnabled = false;
            var addFavoritesButton = new ApplicationBarIconButton()
            {
                IconUri = new Uri("/Assets/Images/AppBar/favs.addto.png", UriKind.Relative),
                Text = "избранное"
            };
            addFavoritesButton.Click += ApplicationBarIconButton_OnClick;
            ApplicationBar.Buttons.Add(addFavoritesButton);

            var refreshButton = new ApplicationBarIconButton()
            {
                IconUri = new Uri("/Assets/Images/AppBar/refresh.png", UriKind.Relative),
                Text = "обновить"
            };
            refreshButton.Click += RefreshButton_OnClick;
            ApplicationBar.Buttons.Add(refreshButton);
        }

        private async Task  StartRequest()
        {
            if (!_requestExecution)
            {
                _requestExecution = true;
                var reqManip = _viewModel.RequestManip;
                if (reqManip.GetType() == typeof(RequestByStudent))
                {
                    SubjectsPanoramaItem.Visibility = Visibility.Collapsed;
                    GroupRatingPanoramaItem.Visibility = Visibility.Collapsed;
                    SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
                    var studentRequest = reqManip as RequestByStudent;
                    _viewModel.StudentNumber = studentRequest.StudentNumber();
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
                    }
                }
                _requestExecution = false;
            }
        }

       

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (isSelectorEnabled)
            {
                InitAppBar();
                e.Cancel = true;
            }
            else
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
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsManager.SendView("Рейтинг студентов");

            var reqManip = NavigationService.GetNavigationData() as RequestManipulation;
            if (reqManip == null) return;
            _viewModel.SetCurrentRequest(reqManip);
            _viewModel.GetRatingFromDb(reqManip);
            RatingPanorama.DefaultItem = GroupRatingPanoramaItem;

            await StartRequest();
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
            var subjLb = sender as ListBox;
            if (subjLb != null && subjLb.SelectedItem != null)
            {
                var selectedItem = (subjLb.SelectedItem as ArrayItemWrapper).Value;
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
                var studentRequest = _viewModel.RequestManipForStudent as RequestByStudent;
                _viewModel.StudentNumber = studentRequest.StudentNumber();
                if (StudentRatingListBox.Items.Count > 0)
                {
                    StudentRatingListBox.ScrollIntoView(StudentRatingListBox.Items.First());
                }
            }
        }

        #endregion
        
        private async void RefreshButton_OnClick(object sender, EventArgs e)
        {
                var item = RatingPanorama.SelectedItem as PanoramaItem;
                if (item == StudentPanoramaItem && _viewModel.RequestManipForStudent != null)
                {
                    await StartRequestByStudent();
                }
                else
                {
                    await StartRequest();
                }
            
        }

        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            if (_viewModel.RequestManipForStudent != null)
            {
                isSelectorEnabled = true;
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
                
                //App.RootFrame.Dispatcher.BeginInvoke(() =>
                //{
                App.ProgressIndicator.IsVisible = false;
                    if (!_viewModel.CheckFavorites(false))
                        MessageBox.Show("Запись уже находится в избранном");
                    else
                        ShowCustomMessageBox(false);
             //   }
               //     );

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
            if (RatingPanorama.SelectedItem == StudentPanoramaItem)
            {
                if (e.Orientation == PageOrientation.LandscapeRight || e.Orientation == PageOrientation.LandscapeLeft)
                {
                    VerticalState.Visibility = Visibility.Collapsed;
                    HorizontalState.Visibility = Visibility.Visible;
                    SystemTray.IsVisible = false;
                    ApplicationBar.IsVisible = false;
                }
                else
                {
                    VerticalState.Visibility = Visibility.Visible;
                    HorizontalState.Visibility = Visibility.Collapsed;
                    SystemTray.IsVisible = true;
                    ApplicationBar.IsVisible = true;
                }
            }
        }

        private async void RatingPanorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var panorama = sender as Panorama;
            if (panorama == null) return;

            if ((panorama.SelectedItem as PanoramaItem) != StudentPanoramaItem) return;
            if (_viewModel.RequestManipForStudent == null || GroupRatingListBox.SelectedIndex == _selectedStudent)
                return;

            _selectedStudent = GroupRatingListBox.SelectedIndex;
            await StartRequestByStudent();
        }

        private async Task StartRequestByStudent()
        {
            if (StudentRatingListBox.Opacity == 1.0d)
            {
                var opacityInSb = FindName("StudentOpacityInSb") as Storyboard;
                if (opacityInSb != null)
                {
                    Storyboard.SetTargetName(opacityInSb, "StudentRatingListBox");
                    opacityInSb.Begin();
                    opacityInSb.Completed += (o, args) => { StudentRatingListBox.Opacity = 0; opacityInSb.Stop(); };
                }
            }

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

            if (StudentRatingListBox.Opacity != 1.0d)
            {
                var opacityOutSb = FindName("StudentOpacityOutSb") as Storyboard;
                if (opacityOutSb != null)
                {
                    Storyboard.SetTargetName(opacityOutSb, "StudentRatingListBox");
                    opacityOutSb.Begin();
                    opacityOutSb.Completed += (o, args) => { StudentRatingListBox.Opacity = 1; opacityOutSb.Stop(); };
                }
            }
        }
    }
}