using System;
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
    //Todo Доделать Контекстное меню
    //Todo Проверить работу с коллекцией избранного
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
            RatingTypePivot.Focus();
        }

        
        #region AppBar

        #region MainAppBar

        private ApplicationBarIconButton _appBarButtonAddFavorite;
        private ApplicationBarIconButton _appBarButtonSelectItems;
        private void InitializeMainAppBar()
        {
            ApplicationBar = new ApplicationBar {IsVisible = true, IsMenuEnabled = true};

            _appBarButtonAddFavorite = new ApplicationBarIconButton(new Uri("/Assets/Images/AppBar/add.png", UriKind.Relative));
            _appBarButtonAddFavorite.Text = "Добавить";
            _appBarButtonAddFavorite.Click += appBarButtonAddFavorite_Click;
            ApplicationBar.Buttons.Add(_appBarButtonAddFavorite);

            if (_viewModel.FavoritesList != null && _viewModel.FavoritesList.Count > 0)
            {
                _appBarButtonSelectItems = new ApplicationBarIconButton(new Uri("/Assets/Images/AppBar/check.png", UriKind.Relative));
                _appBarButtonSelectItems.Text = "Выделить";
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
            NavigationService.Navigate(new Uri(String.Format("/Pages/InputDataPage.xaml?mode={0}", InputDataMode.AddTemplate), UriKind.Relative));
        }

        #endregion

        #region SelectionAppBar

        private ApplicationBarIconButton _appBarButtonDelFavorite;
        private ApplicationBarIconButton _appBarButtonEditFavorite;
        private void InitializeSelectionAppBar()
        {
            ApplicationBar = new ApplicationBar { IsVisible = true, IsMenuEnabled = true };

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
                var collection = FavotitesMultiselectList.SelectedItems as List<FavoritesItem>;
                if (collection != null)
                {
                    var item = collection.First();
                    NavigationService.Navigate(
                    new Uri(String.Format("/Pages/InputDataPage.xaml?mode={0}templateId={1}", InputDataMode.EditTemplate, item.Id), UriKind.Relative));
                }
            }
        }

        private void appBarButtonDeleteFavorite_Click(object sender, EventArgs e)
        {
            if (FavotitesMultiselectList.SelectedItems.Count > 0)
            {
                var selectedCollection = FavotitesMultiselectList.SelectedItems as List<FavoritesItem>;
                if (selectedCollection != null)
                {
                    //Todo удалить из _viewModel.FavoritesItems выделенные элементы
                    //Todo удалить из базы данных
                }
            }
            
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
            InitializeSelectionAppBar();
        }

        private void FavotitesMultiselectList_OnHold(object sender, GestureEventArgs e)
        {
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
            var contextMenu = sender as DependencyObject;
            if (contextMenu != null)
            {
                var favItem = ContextMenuService.GetContextMenu(contextMenu).Owner as LongListMultiSelectorItem;
                if (favItem != null)
                {
                    //Todo получить выбранный объект и удалить его
                }
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
            var contextMenu = sender as DependencyObject;
            if (contextMenu != null)
            {
                var favItem = ContextMenuService.GetContextMenu(contextMenu).Owner as LongListMultiSelectorItem;
                if (favItem != null)
                {
                    //Todo получить выбранный объект и редактировать его
                }
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
                    new Uri(String.Format("/Pages/InputDataPage.xaml?mode={0}templateId={1}", InputDataMode.UseTemplate, item.Id), UriKind.Relative));
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