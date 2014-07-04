using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RatingVolsuAPI;

namespace RatingVolsuWP8
{

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
            RatingOfGroupForView = new ObservableCollection<Rating>();
            Subjects = new ObservableCollection<Subject>();
        }

        #region WEB

        public async Task GetWebRatingOfStudent(RequestManipulation requestManip)
        {
            var temp  = await _requestManager.GetRatingOfStudent(requestManip);
            if (temp != null)
            {
                RatingOfStudent = temp;
                RatingOfStudent = new ObservableCollection<Rating>(RatingOfStudent.OrderByDescending(x => x.Total));
            }
        }

        public async Task GetWebRatingOfGroup(RequestManipulation requestManip)
        {
            var temp = await _requestManager.GetRatingOfGroup(requestManip);
            if (temp != null)
            {
                Subjects = new ObservableCollection<Subject>(temp.Select(x => x.Subject).Distinct(new SubjectsComparer()));
                RatingOfGroup = temp.AllSubjects();
                RatingOfGroupForView = new ObservableCollection<Rating>(RatingOfGroup.Where(x => x.SubjectId == "000").OrderByDescending(x => x.Total).ToList());
                Subjects.Insert(0, new Subject()
                {
                    Id = "000",
                    Name = "Все предметы"
                });
            }
        }

        #endregion

        #region DataBase

        internal void GetFavoriteItem(string itemId)
        {
            CurrentFavoritesItem = _ratingDb.GetFavoritesItem(itemId);
        }

        public void GetRatingFromDb(RequestManipulation reqManip)
        {
            var listRatings = reqManip.LoadRatingFromDb();
            if (RequestManip.GetType() == typeof(RequestByStudent))
                RatingOfStudent = listRatings;
            else
            {
                Subjects = new ObservableCollection<Subject>(listRatings.Select(x => x.Subject).Distinct(new SubjectsComparer()));
                RatingOfGroup = listRatings.AllSubjects();
                RatingOfGroupForView = new ObservableCollection<Rating>(RatingOfGroup.Where(x => x.SubjectId == "000").OrderByDescending(x => x.Total).ToList());
                if (Subjects.Count !=0) 
                    Subjects.Insert(0, new Subject()
                    {
                        Id = "000",
                        Name = "Все предметы"
                    });
            }
        }
        #endregion

        internal void GetRatingBySubject(int selectedIndex)
        {
            RatingOfGroupForView = new ObservableCollection<Rating>(RatingOfGroup.Where(x => x.SubjectId == Subjects[selectedIndex].Id).OrderByDescending(x => x.Total).ToList());
        }

        public void SetStatisticForRating(Rating curItem)
        {
            int totalCur = Convert.ToInt32(curItem.Total);
                //curItem.Total.Length == 1 ?
                //curItem.Total : curItem.Total.Substring(0, 2).Replace("(", ""));

            // Generate BallsToNextPlace
            var idx = RatingOfGroupForView.IndexOf(RatingOfGroup.First(x => x.Id == curItem.Id));
            if (idx == 0)
            {
                curItem.BallsToNextPlace = "0";
                curItem.BallsToFirstPlace = "0";
                return;
            }
            if (!String.IsNullOrEmpty(RatingOfGroupForView[idx - 1].Total))
            {
                int totalPred = Convert.ToInt32(RatingOfGroupForView[idx - 1].Total);
                    //RatingOfGroupForView[idx - 1].Total.Length == 1 ? 
                    //RatingOfGroupForView[idx - 1].Total : RatingOfGroupForView[idx - 1].Total.Substring(0, 2).Replace("(", ""));
                curItem.BallsToNextPlace = String.Format("+{0}", totalPred - totalCur);
            }
            else
            {
                curItem.BallsToNextPlace = String.Empty;
            }
            // Generate BallsToFirstPlace
            int totalFirst = Convert.ToInt32(RatingOfGroupForView.First().Total);//RatingOfGroupForView.First().Total.Length == 1 ? RatingOfGroupForView.First().Total : RatingOfGroupForView.First().Total.Substring(0, 2).Replace("(", ""));
            curItem.BallsToFirstPlace = String.Format("+{0}", totalFirst - totalCur);
        }

        internal void SaveFavorites(bool p, string name)
        {
            _ratingDb.SaveFavorites(p ? RequestManipForStudent : RequestManip, name);
        }

        public bool CheckFavorites(bool p)
        {
            return _ratingDb.CheckFavorites(p ? RequestManipForStudent : RequestManip);
        }

        internal void SetCurrentRequest(RequestManipulation reqManip)
        {
            RequestManip = reqManip;
        }
    }
}
