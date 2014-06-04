using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            _viewModel = new RatingViewModel();
            DataContext = _viewModel;
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
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
                    StudentPanoramaItem.Visibility = Visibility.Collapsed;
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

        }

        private Rating Head;
        private void RatingPage_OnOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (_viewModel.RequestManipForStudent != null)
            {
                if (e.Orientation == PageOrientation.LandscapeRight || e.Orientation == PageOrientation.LandscapeLeft)
                {
                    VerticalState.Visibility = Visibility.Collapsed;
                    HorizontalState.Visibility = Visibility.Visible;
                    SystemTray.IsVisible = false;
                    if (Head == null)
                    {
                        Head = new Rating
                        {
                            Subject = new Subject()
                            {
                                Name = "ПРЕДМЕТЫ"
                            },
                            Att1 = "1 модуль",
                            Att2 = "2 модуль",
                            Att3 = "3 модуль",
                            Sum = "сумма",
                            Exam = "экзамен",
                            Total = "итог"
                        };
                        _viewModel.RatingOfStudent.Insert(0,Head);
                    }
                    
                }
                else
                {
                    if (_viewModel.RatingOfStudent.Contains(Head))
                        _viewModel.RatingOfStudent.Remove(Head);
                    VerticalState.Visibility = Visibility.Visible;
                    HorizontalState.Visibility = Visibility.Collapsed;
                    SystemTray.IsVisible = true;
                }
            }
        }
    }
}