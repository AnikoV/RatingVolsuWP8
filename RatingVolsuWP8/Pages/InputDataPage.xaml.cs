using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Windows.Networking.Connectivity;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using RatingVolsuAPI;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace RatingVolsuWP8
{
    public partial class InputDataPage : PhoneApplicationPage
    {
        readonly InputDataViewModel _viewModel;
        public InputDataPage()
        {
            InitializeComponent();
            _viewModel = new InputDataViewModel();
            DataContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Парсим мод
            string mode, templateId;
            if (NavigationContext.QueryString.TryGetValue("mode", out mode))
            {
                _viewModel.CurrentInputMode = (InputDataMode) Enum.Parse(typeof (InputDataMode), mode);
            }
            //Парсим тип рейтинга инициализируем тип запроса
            string type;
            if(NavigationContext.QueryString.TryGetValue("type", out type))
                _viewModel.CurrentRatingType = (RatingType)Enum.Parse(typeof(RatingType), type);
            else
                if (NavigationContext.QueryString.TryGetValue("templateId", out templateId))
                    _viewModel.GetFavoriteItem(templateId);
                    
            if (_viewModel.CurrentRatingType == RatingType.RatingOfStudent)
            {
                _viewModel.RequestManip = new RequestByStudent();
            }
            else
            {
                _viewModel.RequestManip = new RequestByGroup();
            }
            App.InitProgressIndicator(true,"Загрузка институтов...",this);
            if (await App.IsInternetAvailable())
            {
                await _viewModel.GetFacults();
                App.ProgressIndicator.IsVisible = false;
            }
            else
            {
                App.ProgressIndicator.IsVisible = false;
                MessageBox.Show("К сожалению, соединение с интернетом недоступно.");
                return;
            }
            
        }

        #region OnSelectionChanged

        private async void InstituteListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                var selectedIndex = listBox.SelectedIndex;
                if (selectedIndex == -1) 
                    return;

                App.InitProgressIndicator(true, "Загрузка групп...", this);
                if (await App.IsInternetAvailable())
                {
                    ApplicationBar.IsVisible = false;
                    _viewModel.Groups.Clear(); _viewModel.Semesters.Clear(); _viewModel.Students.Clear();
                    await _viewModel.GetGroups(selectedIndex);
                    App.ProgressIndicator.IsVisible = false;
                }
                else
                {
                    App.ProgressIndicator.IsVisible = false;
                    MessageBox.Show("К сожалению, соединение с интернетом недоступно.");
                    return;
                }
                int indexItem = InputDataPanorama.Items.IndexOf(GroupPanoramaItem);
                InputDataPanorama.Items.RemoveAt(indexItem);
                InputDataPanorama.Items.Insert(indexItem, GroupPanoramaItem);
                GroupPanoramaItem.Visibility = Visibility.Visible;
            }
        }

        private async void GroupListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                int selectedIndex = listBox.SelectedIndex;

                if(selectedIndex == -1)
                    return;
                App.InitProgressIndicator(true, "Загрузка семестров...", this);
                if (await App.IsInternetAvailable())
                {
                    ApplicationBar.IsVisible = false;
                    _viewModel.Semesters.Clear(); _viewModel.Students.Clear();
                    #region Generate Semesters

                    var selectedGroupName = _viewModel.Groups[selectedIndex].Name;
                    _viewModel.RequestManip.GroupId = _viewModel.Groups[selectedIndex].Id;

                    int introYear = Convert.ToInt32(selectedGroupName.Substring(selectedGroupName.Length - 3, 2));
                    var semesterList = await _viewModel.GetSemestrList(_viewModel.RequestManip.GroupId);
                    int temp = introYear;
                    _viewModel.Semesters.Clear();

                    for (int i = 0; i < semesterList.Count; i++)
                    {
                        Semester semestr;
                        if (temp < 9)
                        {
                            semestr = new Semester
                            {
                                Number = semesterList[i] + " семестр",
                                YearsPeriod = String.Format("200{0} - 200{1}", temp, temp + 1)
                            };
                        }
                        else if (temp == 9)
                        {
                            semestr = new Semester
                            {
                                Number = semesterList[i] + " семестр",
                                YearsPeriod = String.Format("200{0} - 20{1}", temp, temp + 1)
                            };
                        }
                        else
                        {
                            semestr = new Semester
                            {
                                Number = semesterList[i] + " семестр",
                                YearsPeriod = String.Format("20{0} - 20{1}", temp, temp + 1)
                            };
                        }


                        if (i%2 != 0)
                            temp++;

                        Debug.WriteLine(String.Format("Semestr: " + semestr.Number + " | " + semestr.YearsPeriod));
                        _viewModel.Semesters.Add(semestr);
                    }

                    #endregion
                    App.ProgressIndicator.IsVisible = false;
                }
                else
                {
                    App.ProgressIndicator.IsVisible = false;
                    MessageBox.Show("К сожалению, соединение с интернетом недоступно.");
                    return;
                }

                int indexItem = InputDataPanorama.Items.IndexOf(SemesterPanoramaItem);
                InputDataPanorama.Items.RemoveAt(indexItem);
                InputDataPanorama.Items.Insert(indexItem, SemesterPanoramaItem);
                SemesterPanoramaItem.Visibility = Visibility.Visible;
            }
        }

        private async void SemestrListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var SelectedIndex = SemestrListBox.SelectedIndex;
            if (SelectedIndex == -1) 
                return;
            _viewModel.RequestManip.Semestr = (SelectedIndex + 1).ToString();
            if (_viewModel.RequestManip.GetType() == typeof(RequestByStudent))
            {
                ApplicationBar.IsVisible = false;
                _viewModel.Students.Clear();
                App.InitProgressIndicator(true, "Загрузка номеров зачеток...", this);
                if (await App.IsInternetAvailable())
                {
                    await _viewModel.GetStudents();
                    App.ProgressIndicator.IsVisible = false;
                }
                else
                {
                    App.ProgressIndicator.IsVisible = false;
                    MessageBox.Show("К сожалению, соединение с интернетом недоступно.");
                    return;
                }

                int indexItem = InputDataPanorama.Items.IndexOf(ZachetkaPanoramaItem);
                InputDataPanorama.Items.RemoveAt(indexItem);
                InputDataPanorama.Items.Insert(indexItem, ZachetkaPanoramaItem);
                ZachetkaPanoramaItem.Visibility = Visibility.Visible;

            }
            else
            {
                ApplicationBar.IsVisible = true;
            }
        }

        private void ZachetkaListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var SelectedIndex = ZachetkaListBox.SelectedIndex;
            if (SelectedIndex == -1) return;
            ApplicationBar.IsVisible = true;
            var reqManipStud = (RequestByStudent)_viewModel.RequestManip;
            reqManipStud.StudentId = _viewModel.Students[SelectedIndex].Id;
        }
        #endregion

        #region AppBar

        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            switch (_viewModel.CurrentInputMode)
            {
                case InputDataMode.Standart:
                    //Todo уходим на рейтинг
                    NavigationService.Navigate(new Uri(String.Format("/Pages/RatingPage.xaml?type={0}",_viewModel.CurrentRatingType), UriKind.Relative), _viewModel.RequestManip);
                    break;
                case InputDataMode.AddTemplate:
                    ShowCustumMessageBox();
                    break;
                case InputDataMode.EditTemplate:
                    _viewModel.EditFavorites();
                    NavigationService.Navigate(new Uri("/MainPage.xaml?tofavorites=true", UriKind.Relative));
                    break;
            }
            //Todo запрос на рейтинг и переход на страницу рейтинга
        }

        private void ShowCustumMessageBox()
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
                        _viewModel.SaveFavorites(textBox.Text);
                        NavigationService.Navigate(new Uri("/MainPage.xaml?tofavorites=true", UriKind.Relative));
                        break;
                    default: break;
                }
            };

            cmBox.Show();
        }

        #endregion

        
    }
}