using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using RatingVolsuAPI;
using RatingVolsuWP8.Resources;

namespace RatingVolsuWP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();
            InitializeAppBar();
            DataContext = App.ViewModel;

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
            App.ViewModel.GroupId = App.ViewModel.groupCollection[SelectedIndex].Id;
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
            App.ViewModel.Semestr = (SelectedIndex + 1).ToString();
            App.ViewModel.GetStudents(SelectedIndex);
        }

        private void StudentList_Tap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = StudentList.SelectedIndex;
            if (SelectedIndex == -1) return;
            ApplicationBar.IsVisible = true;
            App.ViewModel.StudentId = App.ViewModel.studentCollection[SelectedIndex].Id;
            
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
            var appBarButtonSave =
                new ApplicationBarIconButton(new Uri("/Images/AppBar/check.png", UriKind.Relative));
            appBarButtonSave.Text = "Сохранить";
            appBarButtonSave.Click += appBarButtonSave_Click;
            ApplicationBar.Buttons.Add(appBarButtonSave);
        }

        private void appBarButtonSave_Click(object sender, EventArgs e)
        {
            App.ViewModel.SaveChangesRatingtoDb(App.CurrentFavorites);
            NavigationService.Navigate(new Uri("/TestPage.xaml?=" + App.ViewModel.FacultId, UriKind.Relative));

        }

        private void appBarButtonOK_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/RequestDataView.xaml?facult=" + App.ViewModel.FacultId +
                                                                                    "&group=" + App.ViewModel.GroupId +
                                                                                    "&semestr=" + App.ViewModel.Semestr +
                                                                                    "&student=" + App.ViewModel.StudentId, UriKind.Relative));
        }
        #endregion
    }
}