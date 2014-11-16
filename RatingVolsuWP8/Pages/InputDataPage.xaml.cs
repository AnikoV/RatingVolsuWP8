using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using RatingVolsuAPI;

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
            AnalyticsManager.SendView("Ввод данных");
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
                
                if (GroupPanoramaItem.Opacity == 1.0d)
                {
                    var opacityInSb = FindName("GroupOpacityInSb") as Storyboard;
                    if (opacityInSb != null)
                    {
                        Storyboard.SetTargetName(opacityInSb, "GroupPanoramaItem");
                        opacityInSb.Begin();
                        opacityInSb.Completed += (o, args) => { GroupPanoramaItem.Opacity = 0; opacityInSb.Stop(); };
                    }
                }

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
                
                if (GroupPanoramaItem.Opacity != 1.0d)
                {
                    var opacityOutSb = FindName("GroupOpacityOutSb") as Storyboard;
                    if (opacityOutSb != null)
                    {
                        Storyboard.SetTargetName(opacityOutSb, "GroupPanoramaItem");
                        opacityOutSb.Begin();
                        opacityOutSb.Completed += (o, args) => { GroupPanoramaItem.Opacity = 1; opacityOutSb.Stop(); };
                    }
                }
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

                if (SemesterPanoramaItem.Opacity == 1.0d)
                {
                    var opacityInSb = FindName("SemesterOpacityInSb") as Storyboard;
                    if (opacityInSb != null)
                    {
                        Storyboard.SetTargetName(opacityInSb, "SemesterPanoramaItem");
                        opacityInSb.Begin();
                        opacityInSb.Completed += (o, args) => { SemesterPanoramaItem.Opacity = 0; opacityInSb.Stop(); };
                    }
                }

                if (await App.IsInternetAvailable())
                {
                    ApplicationBar.IsVisible = false;
                    _viewModel.Semesters.Clear(); _viewModel.Students.Clear();
                    #region Generate Semesters

                    _viewModel.RequestManip.GroupId = _viewModel.Groups[selectedIndex].Id;

                    int introYear = Convert.ToInt32(_viewModel.Groups[selectedIndex].Year);
                    var semesterList = await _viewModel.GetSemestrList(_viewModel.RequestManip.GroupId);
                    int temp = introYear;
                    _viewModel.Semesters.Clear();

                    for (int i = 0; i < semesterList.Count; i++)
                    {
                        Semester semestr;
                        semestr = new Semester
                        {
                            Number = semesterList[i],
                            YearsPeriod = String.Format("{0} - {1}", temp, temp + 1)
                        };
                        
                        if (i%2 != 0)
                            temp++;

                        Debug.WriteLine(String.Format("Semestr: " + semestr.NumberText + " | " + semestr.YearsPeriod));
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

                if (SemesterPanoramaItem.Opacity != 1.0d)
                {
                    var opacityOutSb = FindName("SemesterOpacityOutSb") as Storyboard;
                    if (opacityOutSb != null)
                    {
                        Storyboard.SetTargetName(opacityOutSb, "SemesterPanoramaItem");
                        opacityOutSb.Begin();
                        opacityOutSb.Completed += (o, args) => { SemesterPanoramaItem.Opacity = 1; opacityOutSb.Stop(); };
                    }
                }
            }
        }

        private async void SemestrListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var SelectedIndex = SemestrListBox.SelectedIndex;
            Debug.WriteLine("Changing semester with index" + SelectedIndex);
            if (SelectedIndex == -1) 
                return;
            _viewModel.RequestManip.Semestr = _viewModel.Semesters[SelectedIndex].Number;
            if (_viewModel.RequestManip.GetType() == typeof(RequestByStudent))
            {
                ApplicationBar.IsVisible = false;
                _viewModel.Students.Clear();

                App.InitProgressIndicator(true, "Загрузка номеров зачеток...", this);
                if (ZachetkaPanoramaItem.Opacity == 1.0d)
                {
                    var opacityInSb = FindName("ZachetkaOpacityInSb") as Storyboard;
                    if (opacityInSb != null)
                    {
                        Storyboard.SetTargetName(opacityInSb, "ZachetkaPanoramaItem");
                        opacityInSb.Begin();
                        opacityInSb.Completed += (o, args) => { ZachetkaPanoramaItem.Opacity = 0; opacityInSb.Stop(); };
                    }
                }

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

                if (ZachetkaPanoramaItem.Opacity != 1.0d)
                {
                    var opacityOutSb = FindName("ZachetkaOpacityOutSb") as Storyboard;
                    if (opacityOutSb != null)
                    {
                        Storyboard.SetTargetName(opacityOutSb, "ZachetkaPanoramaItem");
                        opacityOutSb.Begin();
                        opacityOutSb.Completed += (o, args) => { ZachetkaPanoramaItem.Opacity = 1; opacityOutSb.Stop(); };
                    }
                }
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
                    NavigationService.Navigate(new Uri(String.Format("/Pages/RatingPage.xaml?type={0}",_viewModel.CurrentRatingType), UriKind.Relative), _viewModel.RequestManip);
                    break;
                case InputDataMode.AddTemplate:
                    if (!_viewModel.CheckFavorites())
                        MessageBox.Show("Запись уже находится в избранном");
                    else
                        ShowCustomMessageBox();
                    break;
                case InputDataMode.EditTemplate:
                    _viewModel.EditFavorites();
                    break;
            } 
        }

        private void ShowCustomMessageBox()
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
                        if (textBox.Text.Length <= 12)
                        {
                            _viewModel.SaveFavorites(textBox.Text);
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
                        }
                        else
                        {
                            MessageBox.Show("Название слишком длинное");
                            ShowCustomMessageBox();
                        }
                        break;
                    default: break;
                }
            };

            cmBox.Show();
        }

        #endregion

        
    }
}