using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
        private readonly RequestManager _requestManager;
        private readonly RatingDatabase _ratingDb;
        public FavoritesItem CurrentFavoritesItem;
        
        //Properties
        public int[] Place { get; set; }
        public ObservableCollection<Rating> RatingOfStudent { get; set; }
        public ObservableCollection<Rating> RatingOfGroup { get; set; }
        public ObservableCollection<Rating> RatingOfGroupForView { get; set; }
        public ObservableCollection<Subject> Subjects { get; set; }
        public string BallsToNextPlace { get; set; }

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

        public string GetStatisticForPred(int itemId, string curTotal)
        {
            string result;
            int totalCur = Convert.ToInt32(curTotal.Substring(0, 2).Replace("(", ""));
            //Generate 1st
            var idx = RatingOfGroupForView.IndexOf(RatingOfGroup.First(x => x.Id == itemId));
            if (idx == 0)
                return null;
            if (!String.IsNullOrEmpty(RatingOfGroupForView[idx - 1].Total))
            {
                int totalPred = Convert.ToInt32(RatingOfGroupForView[idx - 1].Total.Substring(0, 2).Replace("(", ""));
                result = (totalPred - totalCur).ToString();
            }
            else
            {
                result = String.Empty;
            }
            return result;
        }

        public string GetStatisticForFirst(int itemId, string curTotal)
        {
            int totalCur = Convert.ToInt32(curTotal.Substring(0, 2).Replace("(", ""));
            int totalFirst = Convert.ToInt32(RatingOfGroupForView.First().Total.Substring(0, 2).Replace("(", ""));
            string result = (totalFirst - totalCur).ToString();

            return result;
        }
    }
}
