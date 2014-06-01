using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatingVolsuAPI;
using RatinVolsuAPI;

namespace RatingVolsuWP8
{
    public class ItemsComparer : IEqualityComparer<Rating>
    {
        public bool Equals(Rating x, Rating y)
        {
            return x.StudentId == y.StudentId;
        }

        public int GetHashCode(Rating obj)
        {
            return obj.StudentId.GetHashCode();
        }
    }

    public class SubjectsComparer : IEqualityComparer<Subject>
    {
        public bool Equals(Subject x, Subject y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Subject obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public class RatingViewModel : PropertyChangedBase
    {
        //Settings
        public InputDataMode CurrentInputMode;
        public RatingType CurrentRatingType;
        public RequestManipulation RequestManip;
        private readonly RequestManager _requestManager;
        private readonly RatingDatabase _ratingDb;
        public FavoritesItem CurrentFavoritesItem;
        

        //Properties
        public ObservableCollection<int> Place { get; set; }
        public ObservableCollection<Rating> RatingOfStudent { get; set; }
        public ObservableCollection<Rating> RatingOfGroup { get; set; }
        public ObservableCollection<Rating> RatingOfGroupForView { get; set; }
        public ObservableCollection<Subject> Subjects { get; set; }
        

        public RatingViewModel()
        {
            _ratingDb = new RatingDatabase(App.DbConnectionString);
            _requestManager = new RequestManager();
            RatingOfStudent = new ObservableCollection<Rating>();
            Subjects = new ObservableCollection<Subject>();
        }

        #region WEB

        public async Task GetWebRatingOfStudent(RequestManipulation requestManip)
        {
            RequestManip = requestManip;
            RatingOfStudent = await _requestManager.GetRatingOfStudent(RequestManip);
        }

        public async Task GetWebRatingOfGroup(RequestManipulation requestManip)
        {
            RequestManip = requestManip;
            RatingOfGroup = await _requestManager.GetRatingOfGroup(RequestManip);
            RatingOfGroupForView = new ObservableCollection<Rating>(RatingOfGroup.Distinct(new ItemsComparer()).OrderByDescending(x => x.Total));
            Place = new ObservableCollection<int>();
            for (int i = 0; i < RatingOfGroupForView.Count; i++)
            {
                Place.Add(i+1);
            }
            Subjects = new ObservableCollection<Subject>(RatingOfGroup.Select(x => x.Subject).Distinct(new SubjectsComparer()));
        }
        #endregion

        #region DataBase

        internal void GetFavoriteItem(string itemId)
        {
            CurrentFavoritesItem = _ratingDb.GetFavoritesItem(Convert.ToInt32(itemId));
        }

        #endregion

        public void GetRatingFromDb(RequestManipulation reqManip)
        {
            RequestManip = reqManip;
            var listRatings = RequestManip.LoadRatingFromDb();
            if (RequestManip.GetType() == typeof(RequestByStudent))
                RatingOfStudent = listRatings;
            else
            {
                RatingOfGroup = listRatings;
                //ratingOfGroupForView =
                //    new ObservableCollection<Rating>(ratingOfGroupCollection.Distinct(new ItemsComparer()).ToList());

                Subjects = new ObservableCollection<Subject>(RatingOfGroup.Select(x => x.Subject));
                    //.Distinct(new SubjectsComparer()));
            }
        }
    }
}
