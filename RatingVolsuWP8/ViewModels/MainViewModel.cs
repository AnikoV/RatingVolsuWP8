using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatingVolsuAPI;

namespace RatingVolsuWP8
{
    public class MainViewModel : PropertyChangedBase
    {
        public string VolsuReview { get; set; }
        private RatingDatabase ratingDb = new RatingDatabase(App.DbConnectionString);
        public ObservableCollection<FavoritesItem> FavoritesList { get; set; }
        public MainViewModel()
        {
            VolsuReview = "ВолГУ – университет, известный в стране и за рубежом качеством образования, высоким научным потенциалом, инновационными проектами, активной социальной позицией.";
            GetFavoritesFromDb();
        }
        public void GetFavoritesFromDb()
        {
            FavoritesList = new ObservableCollection<FavoritesItem>(from FavoritesItem item in ratingDb.Favorites select item);
        }

        internal void DeleteFavoriteItems(System.Collections.IList selectedCollection)
        {
            foreach (var favoritesItem in selectedCollection)
            {
                ratingDb.DeleteFavorites((FavoritesItem)favoritesItem);
            }
            FavoritesList = new ObservableCollection<FavoritesItem>(from FavoritesItem item in ratingDb.Favorites select item);
        }
    }
}
