using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RatingVolsuAPI;


namespace RatingVolsuWP8
{
    public partial class RequestDataView : PhoneApplicationPage
    {
        string FacultId;
        string GroupId;
        string Semestr;
        string StudentId;
        private RequestDataViewModel _viewModel;

        public RequestDataView()
        {
            InitializeComponent();
            _viewModel = new RequestDataViewModel();
            DataContext = _viewModel;
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationContext.QueryString.TryGetValue("facult", out FacultId);
            NavigationContext.QueryString.TryGetValue("group", out GroupId);
            NavigationContext.QueryString.TryGetValue("semestr", out Semestr);
            NavigationContext.QueryString.TryGetValue("student", out StudentId);
            _viewModel.GetRatingOfStudent(FacultId, GroupId, Semestr, StudentId);
        }

        private void List_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var SelectedIndex = List.SelectedIndex;
            if (SelectedIndex == -1) return;
            App.RootFrame.Dispatcher.BeginInvoke(new Action<Rating>(c =>
            {
               
            }), _viewModel.ratingCollection[SelectedIndex]);
        }
    }
}