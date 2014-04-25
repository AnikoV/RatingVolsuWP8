using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatingVolsuAPI;

namespace RatingVolsuWP8
{
    class TestViewModel: PropertyChangedBase
    {
        private RatingDatabase rating;
        private ObservableCollection<Facult> _facultCollection;
        public ObservableCollection<Facult> facultCollection
        {
            get { return _facultCollection; }
            set
            {
                _facultCollection = value;
                RaisePropertyChanged("facultCollection");
            }
        }

        private ObservableCollection<Group> _groupCollection;
        public ObservableCollection<Group> groupCollection
        {
            get { return _groupCollection; }
            set
            {
                _groupCollection = value;
                RaisePropertyChanged("groupCollection");
            }
        }

        private ObservableCollection<Group> _grCollection;
        public ObservableCollection<Group> grCollection
        {
            get { return _grCollection; }
            set
            {
                _grCollection = value;
                RaisePropertyChanged("grCollection");
            }
        }

        private ObservableCollection<FavoritesItem> _favoritesCollection;
        public ObservableCollection<FavoritesItem> favoritesCollection
        {
            get { return _favoritesCollection; }
            set
            {
                _favoritesCollection = value;
                RaisePropertyChanged("favoritesCollection");
            }
        }

        public string FacultId { get; set; }

        public TestViewModel()
        {
          
            rating = new RatingDatabase(App.DbConnectionString);

            if (rating.DatabaseExists() != false)
            {
                var facults = from Facult item in rating.Facults
                                select item;
                facultCollection = new ObservableCollection<Facult>(facults);

                var groups = from Group item in rating.Groups
                             select item;
                groupCollection = new ObservableCollection<Group>(groups);

                var favorites = from FavoritesItem item in rating.Favorites
                              select item;
                favoritesCollection = new ObservableCollection<FavoritesItem>(favorites);

            }

            grCollection = new ObservableCollection<Group>(Manager.gr);
            var FacultName = groupCollection[0].Facult.Name;
        }
    }
}
