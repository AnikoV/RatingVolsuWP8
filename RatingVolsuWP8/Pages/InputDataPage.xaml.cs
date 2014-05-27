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
using RatinVolsuAPI;
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
            string mode;
            // Парсим мод
            if (NavigationContext.QueryString.TryGetValue("mode", out mode))
            {
                _viewModel.CurrentInputMode = (InputDataMode) Enum.Parse(typeof (InputDataMode), mode);
            }
            //Парсим тип рейтинга инициализируем тип запроса
            string type;
            if(NavigationContext.QueryString.TryGetValue("type", out type))
            {
                _viewModel.CurrentRatingType = (RatingType)Enum.Parse(typeof(RatingType), type);
                if (_viewModel.CurrentRatingType == RatingType.RatingOfStudent)
                {
                    _viewModel.RequestManip = new RequestByStudent();
                }
                else
                {
                    _viewModel.RequestManip = new RequestByGroup();
                }
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

                //InputDataPanorama.SetValue(Panorama.SelectedItemProperty, InputDataPanorama.Items[1]);
                //SlidePanorama(InputDataPanorama);
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
                    #region Generate Semesters

                    var selectedGroupName = _viewModel.Groups[selectedIndex].Name;
                    int introYear = Convert.ToInt32(selectedGroupName.Substring(selectedGroupName.Length - 3, 2));
                    _viewModel.RequestManip.GroupId = _viewModel.Groups[selectedIndex].Id;
                    int semestrCount = _viewModel.GetSemestrCount();
                    int temp = introYear;
                    _viewModel.Semesters.Clear();
                    for (int i = 0; i < semestrCount; i++)
                    {
                        var semestr = new Semester();
                        semestr.Number = (i + 1).ToString(CultureInfo.InvariantCulture) + " семестр";
                        semestr.YearsPeriod = String.Format("20{0} - 20{1}", temp, temp + 1);
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
                App.InitProgressIndicator(true, "Загрузка институтов...", this);
                if (await App.IsInternetAvailable())
                {
                    await _viewModel.GetStudents(SelectedIndex);
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
                ApplicationBar.IsVisible = true;
            }
        }

        private void SlidePanorama(Panorama pan)
        {
            var panWrapper = VisualTreeHelper.GetChild(pan, 0) as FrameworkElement;
            var panTitle = VisualTreeHelper.GetChild(panWrapper, 1) as FrameworkElement;
            //Get the panorama layer to calculate all panorama items size     
            var panLayer = VisualTreeHelper.GetChild(panWrapper, 2) as FrameworkElement;
            //Get the title presenter to calculate the title size     
            var panTitlePresenter =
                VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(panTitle, 0) as FrameworkElement, 1) as
                    FrameworkElement;
            //Current panorama item index     
            int curIndex = pan.SelectedIndex;
            //Get the next of next panorama item     
            var third =
                VisualTreeHelper.GetChild(pan.Items[curIndex + 1] as PanoramaItem, 0) as
                    FrameworkElement;
            //Be sure the RenderTransform is TranslateTransform     
            if (!(pan.RenderTransform is TranslateTransform) || !(panTitle.RenderTransform is TranslateTransform))
            {
                pan.RenderTransform = new TranslateTransform();
                panTitle.RenderTransform = new TranslateTransform();
            }
            //Increase width of panorama to let it render the next slide (if not, default panorama is 480px and the null area appear if we transform it)     
            pan.Width = 960;
            //Animate panorama control to the right     
            var sb = new Storyboard();
            var a = new DoubleAnimation
            {
                From = 0,
                To = -(pan.Items[curIndex] as PanoramaItem).ActualWidth,
                Duration = new Duration(TimeSpan.FromMilliseconds(700)),
                EasingFunction = new CircleEase()
            };
            //Animate the x transform to a width of one item    
            //This is default panorama easing effect    
            sb.Children.Add(a);
            Storyboard.SetTarget(a, pan.RenderTransform);
            Storyboard.SetTargetProperty(a, new PropertyPath(TranslateTransform.XProperty));
            //Animate panorama title separately     
            var aTitle = new DoubleAnimation
            {
                From = 0,
                To = (panLayer.ActualWidth - panTitlePresenter.ActualWidth)/(pan.Items.Count - 1)*1.5,
                Duration = a.Duration,
                EasingFunction = a.EasingFunction
            };
            //Calculate where should the title animate to     
            //This is default panorama easing effect     sb.Children.Add(aTitle);
            Storyboard.SetTarget(aTitle, panTitle.RenderTransform);
            Storyboard.SetTargetProperty(aTitle, new PropertyPath(TranslateTransform.XProperty));
            //Start the effect     
            sb.Begin();
            //After effect completed, we change the selected item   
            a.Completed += (obj, args) =>
            {
                //Reset panorama width         
                pan.Width = 480;
                //Change the selected item        
                (pan.Items[curIndex] as PanoramaItem).Visibility = Visibility.Collapsed;
                pan.SetValue(Panorama.SelectedItemProperty, pan.Items[(curIndex + 1)%pan.Items.Count]);
                pan.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                (pan.Items[curIndex] as PanoramaItem).Visibility = Visibility.Visible;
                //Reset panorama render transform         
                (pan.RenderTransform as TranslateTransform).X = 0;
                //Reset title render transform         
                (panTitle.RenderTransform as TranslateTransform).X = 0;
                //Because of the next of next item will be load after we change the selected index to next item        
                //I do not want it appear immediately without any effect, so I create a custom effect for it         
                if (!(third.RenderTransform is TranslateTransform))
                {
                    third.RenderTransform = new TranslateTransform();
                }
                Storyboard sb2 = new Storyboard();
                var aThird = new DoubleAnimation()
                {
                    From = 100,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromMilliseconds(300))
                };
                sb2.Children.Add(aThird);
                Storyboard.SetTarget(aThird, third.RenderTransform);
                Storyboard.SetTargetProperty(aThird, new PropertyPath(TranslateTransform.XProperty));
                sb2.Begin();
            };
        }

        #endregion

        #region AppBar



        #endregion

        
    }
}