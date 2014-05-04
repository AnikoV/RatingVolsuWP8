using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using RatingVolsuAPI;

namespace ForTesting.Pages
{
    public partial class InputDataPage : PhoneApplicationPage
    {
        public InputDataPage()
        {
            InitializeComponent();
            InitializeAppBar();
            DataContext = App.ViewModel;
            if (RequestInfo.CurrentRatingType == RatingType.RatingOfGroup)
                StudentItem.Visibility = Visibility.Collapsed;
            else
                StudentItem.Visibility = Visibility.Visible;
            bool isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            if (isNetworkAvailable)
            {
                App.ViewModel.GetFacults();
            }
            else
            {
                MessageBox.Show("Нет интернета!");
            }
        }

        private void FacultList_Tap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = FacultList.SelectedIndex;
            if (SelectedIndex == -1) return;
            ApplicationBar.IsVisible = false;
            App.ViewModel.groupCollection.Clear();
            App.ViewModel.studentCollection.Clear();
            SemestrList.Items.Clear();
            App.ViewModel.GetGroups(SelectedIndex);

        }

        private void GroupList_Tap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = GroupList.SelectedIndex;
            if (SelectedIndex == -1) return;
            RequestInfo.GroupId = App.ViewModel.groupCollection[SelectedIndex].Id;
            ApplicationBar.IsVisible = false;
            App.ViewModel.studentCollection.Clear();
            SemestrList.Items.Clear();
            int n = App.ViewModel.GetSemestrCount();
            for (int i = 1; i <= n; i++)
            {
                SemestrList.Items.Add(i.ToString());
            }
        }

        private void Semestr_Tap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = SemestrList.SelectedIndex;
            if (SelectedIndex == -1) return;
            ApplicationBar.IsVisible = false;
            App.ViewModel.studentCollection.Clear();
           RequestInfo.Semestr = (SelectedIndex + 1).ToString();
            if (RequestInfo.CurrentRatingType == RatingType.RatingOfStudent)
            {
                App.ViewModel.GetStudents(SelectedIndex);
            }
            else
            {
                ApplicationBar.IsVisible = true;
            }

        }

        private void StudentList_Tap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = StudentList.SelectedIndex;
            if (SelectedIndex == -1) return;
            ApplicationBar.IsVisible = true;
            RequestInfo.StudentId = App.ViewModel.studentCollection[SelectedIndex].Id;

        }

        #region AppBar

        private void InitializeAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = false;
            ApplicationBar.IsMenuEnabled = true;

            var appBarButtonOK =
                new ApplicationBarIconButton(new Uri("/Images/AppBar/check.png", UriKind.Relative));
            appBarButtonOK.Text = "ОК";
            appBarButtonOK.Click += appBarButtonOK_Click;
            ApplicationBar.Buttons.Add(appBarButtonOK);
        }

        private void appBarButtonOK_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/TestPage.xaml", UriKind.Relative));
        }

        #endregion
    }
}