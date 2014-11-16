using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RatingVolsuAPI;
using System.Threading.Tasks;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace RatingVolsuWP8
{
    public partial class MainPage
    {
        readonly MainViewModel _viewModel;
        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            
            ReviewTextIn.Begin();   // Start Animation
            InitializeMainAppBar();
            if (_viewModel.FavoritesList.Count != 0)
            {
                MainPivot.SelectedItem = MainPivot.Items[1];
                ApplicationBar.IsVisible = true;
            }
            else
                RatingTypePivot.Focus();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AnalyticsManager.SendView("Главный экран");
            if (Info.ToFavoritesPivot)
            {
                MainPivot.SelectedItem = MainPivot.Items[1];
                ApplicationBar.IsVisible = true;
                Info.ToFavoritesPivot = false;
            }
            
            _viewModel.GetFavoritesFromDb();
        }
        #region AppBar

        #region MainAppBar

        private ApplicationBarIconButton _appBarButtonAddFavorite;
        private ApplicationBarIconButton _appBarButtonSelectItems;
        private ApplicationBarIconButton _appBarButtonAddStudent;
        private ApplicationBarIconButton _appBarButtonAddGroup;
        private void InitializeMainAppBar()
        {
            if (ApplicationBar == null)
                ApplicationBar = new ApplicationBar {IsVisible = true, IsMenuEnabled = true, Opacity = 0.5};
            ApplicationBar.Buttons.Clear();
            _appBarButtonAddFavorite = new ApplicationBarIconButton(new Uri("/Assets/Images/AppBar/add.png", UriKind.Relative))
            {
                Text = "Добавить"
            };
            _appBarButtonAddFavorite.Click += appBarButtonAddFavorite_Click;
            ApplicationBar.Buttons.Add(_appBarButtonAddFavorite);

            if (_viewModel.FavoritesList != null && _viewModel.FavoritesList.Count > 0)
            {
                _appBarButtonSelectItems = new ApplicationBarIconButton(new Uri("/Assets/Images/AppBar/check.png", UriKind.Relative))
                {
                    Text = "Выделить"
                };
                _appBarButtonSelectItems.Click += appBarButtonSelectItems_Click;
                ApplicationBar.Buttons.Add(_appBarButtonSelectItems);
            }
        }

        void appBarButtonSelectItems_Click(object sender, EventArgs e)
        {
            InitSelectionMode();// Включить режим выделения
        }

        void appBarButtonAddFavorite_Click(object sender, EventArgs e)
        {
            ApplicationBar.Buttons.Clear();

            _appBarButtonAddStudent = new ApplicationBarIconButton(new Uri("/Assets/Images/AppBar/studentAppBar.png", UriKind.Relative));
            _appBarButtonAddStudent.Text = "Студент";
            _appBarButtonAddStudent.Click += (o, args) =>
            {
                NavigationService.Navigate(
                    new Uri(
                        String.Format("/Pages/InputDataPage.xaml?type={0}&mode={1}", RatingType.RatingOfStudent,
                            InputDataMode.AddTemplate), UriKind.Relative));
                InitializeMainAppBar();
            };
            ApplicationBar.Buttons.Add(_appBarButtonAddStudent);

            _appBarButtonAddGroup = new ApplicationBarIconButton(new Uri("/Assets/Images/AppBar/groupAppBar.png", UriKind.Relative));
            _appBarButtonAddGroup.Text = "Группа";
            _appBarButtonAddGroup.Click += (o, args) =>
            {
                NavigationService.Navigate(
                    new Uri(
                        String.Format("/Pages/InputDataPage.xaml?type={0}&mode={1}", RatingType.RatingOfGroup,
                            InputDataMode.AddTemplate), UriKind.Relative));
                InitializeMainAppBar();
            };
            ApplicationBar.Buttons.Add(_appBarButtonAddGroup);
        }

        #endregion

        #region SelectionAppBar

        private ApplicationBarIconButton _appBarButtonDelFavorite;
        private ApplicationBarIconButton _appBarButtonEditFavorite;
        private void InitializeSelectionAppBar()
        {
            ApplicationBar = new ApplicationBar { IsVisible = true, IsMenuEnabled = true, Opacity = 0.5};

            _appBarButtonDelFavorite = new ApplicationBarIconButton(new Uri("/Assets/Images/AppBar/delete.png", UriKind.Relative));
            _appBarButtonDelFavorite.Text = "Удалить";
            _appBarButtonDelFavorite.Click += appBarButtonDeleteFavorite_Click;
            ApplicationBar.Buttons.Add(_appBarButtonDelFavorite);

            _appBarButtonEditFavorite = new ApplicationBarIconButton(new Uri("/Assets/Images/AppBar/edit.png", UriKind.Relative));
            _appBarButtonEditFavorite.Text = "Редактировать";
            _appBarButtonEditFavorite.Click += appBarButtonEditFavorite_Click;
            ApplicationBar.Buttons.Add(_appBarButtonEditFavorite);
        }
        /// <summary>
        /// Добавляет или убирает кнопку "редактировать" при изменении кол-ва выделенных объектов
        /// </summary>
        private void UpdateSelectionAppBar()
        {
            if (FavotitesMultiselectList.IsSelectionEnabled)
            {
                if (FavotitesMultiselectList.SelectedItems.Count > 1)
                {
                    if (ApplicationBar.Buttons.Contains(_appBarButtonEditFavorite))
                    {
                        ApplicationBar.Buttons.Remove(_appBarButtonEditFavorite);
                    }
                }
                else if (FavotitesMultiselectList.SelectedItems.Count == 1)
                {
                    if (!ApplicationBar.Buttons.Contains(_appBarButtonEditFavorite))
                    {
                        if (_appBarButtonEditFavorite == null)
                        {
                            _appBarButtonEditFavorite = new ApplicationBarIconButton(new Uri("/Assets/Images/AppBar/edit.png", UriKind.Relative));
                            _appBarButtonEditFavorite.Text = "Редактировать";
                            _appBarButtonEditFavorite.Click += appBarButtonEditFavorite_Click;
                        }
                        ApplicationBar.Buttons.Add(_appBarButtonEditFavorite);
                    }
                }
            }
        }

        private void appBarButtonEditFavorite_Click(object sender, EventArgs e)
        {
            if (FavotitesMultiselectList.SelectedItems.Count == 1)
            {
                var collection = FavotitesMultiselectList.SelectedItems;
                if (collection != null)
                {
                    var item = (FavoritesItem)collection[0];
                    ShowCustomMessageBox(item);
                }
            }
            InitializeMainAppBar();
        }

        private void appBarButtonDeleteFavorite_Click(object sender, EventArgs e)
        {
            if (FavotitesMultiselectList.SelectedItems.Count > 0)
            {
                var selectedCollection = FavotitesMultiselectList.SelectedItems;
                if (selectedCollection != null)
                {
                    ShowDeliteMessageBox(selectedCollection);
                }
            }
        }

        private void ShowDeliteMessageBox(System.Collections.IList selectedCollection)
        {
            var cmBox = new CustomMessageBox()
            {
                Message = "Удалить выделенные элементы?",
                LeftButtonContent = "Да",
                RightButtonContent = "Отмена"

            };
            cmBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        _viewModel.DeleteFavoriteItems(selectedCollection);
                        InitializeMainAppBar();
                        break;
                    default: break;
                }
            };

            cmBox.Show();
        }

        private void ShowCustomMessageBox(FavoritesItem favoritesItem)
        {
            var textBox = new TextBox()
            {
                Width = 300,
                Text = favoritesItem.Name,
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
                            _viewModel.EditFavorites(favoritesItem, textBox.Text);
                        }
                        else
                        {
                            MessageBox.Show("Название слишком длинное");
                            ShowCustomMessageBox(favoritesItem);
                        }
                        break;
                    default: break;
                }
            };

            cmBox.Show();
        }

        #endregion

        #endregion

        #region Favorites

        #region FavoritesMultiSelectList

        private void FavotitesMultiselectList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           UpdateSelectionAppBar();
        }

        private void FavotitesMultiselectList_OnIsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (FavotitesMultiselectList.IsSelectionEnabled)
                InitializeSelectionAppBar();
            else
                InitializeMainAppBar();
        }

        private void FavotitesMultiselectList_OnHold(object sender, GestureEventArgs e)
        {
            if (ContextMenuService.GetContextMenu(sender as DependencyObject) != null) 
                ContextMenuService.GetContextMenu(sender as DependencyObject).IsOpen = true;
        }

        #endregion

        #region ContexMenuHandlers

        /// <summary>
        /// удаляет пункт из избранного
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFavorite(object sender, RoutedEventArgs e)
        {
            var selectedCollection = ((FrameworkElement)sender).DataContext as FavoritesItem;
            if (selectedCollection != null)
            {
                ShowDeliteMessageBox(new List<FavoritesItem>(){selectedCollection});
            }
        }

        /// <summary>
        /// Включает режим выделенияобъектов и выделяет выбранный пункт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFavorite(object sender, RoutedEventArgs e)
        {
            var contextMenu = sender as DependencyObject;
            if (contextMenu != null)
            {
                var favItem = ContextMenuService.GetContextMenu(contextMenu).Owner as LongListMultiSelectorItem;
                if (favItem != null)
                {
                    if (!FavotitesMultiselectList.IsSelectionEnabled)
                        FavotitesMultiselectList.IsSelectionEnabled = true;

                    favItem.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// редактирует выбранный пункт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditFavorite(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)sender).DataContext as FavoritesItem;
            if (item != null)
            {
                ShowCustomMessageBox(item);

            }
        }

        #endregion

        /// <summary>
        /// Переход по выбранному шаблону
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemContentTap(object sender, GestureEventArgs e)
        {
            var item = ((FrameworkElement)sender).DataContext as FavoritesItem;
            if (item != null)
                NavigationService.Navigate(
                    new Uri("/Pages/RatingPage.xaml", UriKind.Relative), item.GetRequest());
        }

        /// <summary>
        /// Отменяет выделение, прежде чем уйти со страницы
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (FavotitesMultiselectList.IsSelectionEnabled)
            {
                FavotitesMultiselectList.IsSelectionEnabled = false;
                InitializeMainAppBar();
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Включает выделение
        /// </summary>
        private void InitSelectionMode()
        {
            //FavoritesListBox.SelectionMode = SelectionMode.Multiple;
            FavotitesMultiselectList.IsSelectionEnabled = true;
        }

        #endregion

        #region MiniPivot

        private void Student_OnTap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(
                new Uri(
                    String.Format("/Pages/InputDataPage.xaml?type={0}&mode={1}", RatingType.RatingOfStudent,
                        InputDataMode.Standart), UriKind.Relative));
        }
        private void Group_OnTap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(
                new Uri(
                    String.Format("/Pages/InputDataPage.xaml?type={0}&mode={1}", RatingType.RatingOfGroup,
                        InputDataMode.Standart), UriKind.Relative));
        }

        #endregion

        private void MainPivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mainPivot = sender as Pivot;

            if (mainPivot == null)
                return;

            if (mainPivot.SelectedIndex == 0)
            {
                ApplicationBar.IsVisible = false;
                ReviewTextIn.Begin();
            }
            else
            {
                ApplicationBar.IsVisible = true;
            }
        }

        
    }
}