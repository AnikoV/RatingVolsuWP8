using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RatingVolsuAPI;

namespace RatingVolsuWP8
{
    public class MainViewModel : PropertyChangedBase
    {
        public string VolsuReview { get; set; }

        private RatingDatabase _ratingDb = new RatingDatabase();
        public ObservableCollection<FavoritesItem> FavoritesList { get; set; }
        public MainViewModel()
        {
            VolsuReview = "ВолГУ – университет, известный в стране и за рубежом качеством образования, высоким научным потенциалом, инновационными проектами, активной социальной позицией.";
            GetFavoritesFromDb();
        }
        public void GetFavoritesFromDb()
        {
            FavoritesList = new ObservableCollection<FavoritesItem>(from FavoritesItem item in _ratingDb.Favorites 
                                                                        select item);
        }
        internal void EditFavorites(FavoritesItem favoritesItem, string name)
        {
            _ratingDb.EditFavorites(favoritesItem, name);
        }
        internal void DeleteFavoriteItems(System.Collections.IList selectedCollection)
        {
            foreach (var favoritesItem in selectedCollection)
            {
                _ratingDb.DeleteFavorites((FavoritesItem)favoritesItem);
            }
            _ratingDb = new RatingDatabase();
            GetFavoritesFromDb();
        }
    }
}
