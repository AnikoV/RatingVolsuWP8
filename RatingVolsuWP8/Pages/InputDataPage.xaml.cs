using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
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
            }
            
        }

        #region OnSelectionChanged

        private void InstituteListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

        #region AppBar



        #endregion
    }
}