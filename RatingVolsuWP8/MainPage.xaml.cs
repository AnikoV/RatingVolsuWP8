using System;
using System.Collections.Generic;
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
using RatingVolsuWP8.Resources;

namespace RatingVolsuWP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        private RatingViewModel _viewModel;
        // Конструктор
        public MainPage()
        {
            InitializeComponent();
            _viewModel = new RatingViewModel();
            DataContext = _viewModel;

            //_viewModel.facultCollection.Add(new Facult(){Name = "begin"});
            //FacultList.Items.Add("jhfhgf");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            if (isNetworkAvailable)
            {

                _viewModel.GetFacults();
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
            _viewModel.UpdateFacultList(SelectedIndex);
            _viewModel.GetGroups(SelectedIndex);
        }

        private void GroupList_Tap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = GroupList.SelectedIndex;
            if (SelectedIndex == -1) return;
            _viewModel.UpdateGroupList(SelectedIndex);
            _viewModel.GetStudents(SelectedIndex);

        }

        private void StudentList_Tap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = StudentList.SelectedIndex;
            if (SelectedIndex == -1) return;
            _viewModel.UpdateStudentList(SelectedIndex);
            _viewModel.StudentId = _viewModel.students[SelectedIndex].Id;
            SemestrList.Items.Clear();
            int n = _viewModel.GetSemestrCount();
            for (int i = 1; i <= n; i++)
            {
                SemestrList.Items.Add(i.ToString());
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var sv = new SavedRequest();
            var list = new List<SavedRequest>();
            sv.SetValues(_viewModel);
            list.Add(sv);
            RecentRequests.SaveRequests(list);
            // _viewModel.GetRatingOfStudent();
        }

        private void Semestr_Tap(object sender, GestureEventArgs e)
        {
            var SelectedIndex = SemestrList.SelectedIndex;
            if (SelectedIndex == -1) return;

            _viewModel.Semestr = (SelectedIndex + 1).ToString();
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // Call the method that runs asynchronously.
            string result = await RecentRequests.ReadSavedRequests();
            text.Text = result;
        }

        // The following method runs asynchronously. The UI thread is not
        // blocked during the delay. You can move or resize the Form1 window 
        // while Task.Delay is running.
        public async Task<string> WaitAsynchronouslyAsync()
        {
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream isoStream =
                new IsolatedStorageFileStream("template.json", FileMode.Open, FileAccess.Read, isoFile);
            StreamReader reader = new StreamReader(isoStream);
            var str = await reader.ReadToEndAsync();
            return str;
        }

        // The following method runs synchronously, despite the use of async.
        // You cannot move or resize the Form1 window while Thread.Sleep
        // is running because the UI thread is blocked.
        public async Task<string> WaitSynchronously()
        {
            // Add a using directive for System.Threading.
            Thread.Sleep(10000);
            return "Finished";
        }
    }
}