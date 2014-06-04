using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using RatingVolsuAPI;

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
        public RequestManipulation RequestManipForStudent;
        private readonly RequestManager _requestManager;
        private readonly RatingDatabase _ratingDb;
        public FavoritesItem CurrentFavoritesItem;
        
        //Properties
        public int[] Place { get; set; }
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
            RatingOfStudent = new ObservableCollection<Rating>(RatingOfStudent.OrderByDescending(x => x.Total));
        }

        public async Task GetWebRatingOfGroup(RequestManipulation requestManip)
        {
            RequestManip = requestManip;
            RatingOfGroup = await _requestManager.GetRatingOfGroup(RequestManip);
            RatingOfGroupForView = new ObservableCollection<Rating>(RatingOfGroup.Distinct(new ItemsComparer()).OrderByDescending(x => x.Total));
            Subjects = new ObservableCollection<Subject>(RatingOfGroup.Select(x => x.Subject).Distinct(new SubjectsComparer()));
        }
        #endregion

        #region DataBase

        internal void GetFavoriteItem(string itemId)
        {
            CurrentFavoritesItem = _ratingDb.GetFavoritesItem(Convert.ToInt32(itemId));
        }

        public void GetRatingFromDb(RequestManipulation reqManip)
        {
            RequestManip = reqManip;
            var listRatings = RequestManip.LoadRatingFromDb();
            if (RequestManip.GetType() == typeof(RequestByStudent))
                RatingOfStudent = listRatings;
            else
            {
                RatingOfGroup = new ObservableCollection<Rating>(listRatings);
                Subjects = new ObservableCollection<Subject>(RatingOfGroup.Select(x => x.Subject).Distinct(new SubjectsComparer()));
            }
        }
        #endregion

        internal void GetRatingBySubject(int selectedIndex)
        {
            RatingOfGroupForView = new ObservableCollection<Rating>(RatingOfGroup.Where(x => x.SubjectId == Subjects[selectedIndex].Id).OrderByDescending(x => x.Total).ToList());
        }

        public bool SetStatisticForRating(Rating curItem)
        {
            int totalCur = Convert.ToInt32(
                curItem.Total.Length == 1 ?
                curItem.Total : curItem.Total.Substring(0, 2).Replace("(", ""));

            // Generate BallsToNextPlace
            var idx = RatingOfGroupForView.IndexOf(RatingOfGroup.First(x => x.Id == curItem.Id));
            if (idx == 0)
                return false;
            if (!String.IsNullOrEmpty(RatingOfGroupForView[idx - 1].Total))
            {
                int totalPred = Convert.ToInt32(
                    RatingOfGroupForView[idx - 1].Total.Length == 1 ? 
                    RatingOfGroupForView[idx - 1].Total : RatingOfGroupForView[idx - 1].Total.Substring(0, 2).Replace("(", ""));
                curItem.BallsToNextPlace = String.Format("+{0}", totalPred - totalCur);
            }
            else
            {
                curItem.BallsToNextPlace = String.Empty;
            }
            // Generate BallsToFirstPlace
            int totalFirst = Convert.ToInt32(RatingOfGroupForView.First().Total.Length == 1 ? RatingOfGroupForView.First().Total : RatingOfGroupForView.First().Total.Substring(0, 2).Replace("(", ""));
            curItem.BallsToFirstPlace = String.Format("+{0}", totalFirst - totalCur);

            return true;
        }

        internal void SaveFavorites(bool p, string name)
        {
            _ratingDb.SaveFavorites(p ? RequestManipForStudent : RequestManip, name);
        }

        public bool CheckFavorites(bool p)
        {
            return _ratingDb.CheckFavorites(p ? RequestManipForStudent : RequestManip);
        }
    }
}
