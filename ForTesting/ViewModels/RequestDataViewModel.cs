using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using RatingVolsuAPI;
using RatingVolsuAPI.Base;
using RatinVolsuAPI;

namespace ForTesting
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

    class RequestDataViewModel : PropertyChangedBase
    {
        private ObservableCollection<Rating> _ratingOfGroupCollection;
        public ObservableCollection<Rating> ratingOfGroupCollection
        {
            get { return _ratingOfGroupCollection; }
            set
            {
                _ratingOfGroupCollection = value;
                RaisePropertyChanged("ratingCollection");
            }
        }

        private ObservableCollection<Rating> _ratingOfGroupForView;
        public ObservableCollection<Rating> ratingOfGroupForView
        {
            get { return _ratingOfGroupForView; }
            set
            {
                _ratingOfGroupForView = value;
                RaisePropertyChanged("ratingOfGroupForView");
            }
        }

        private ObservableCollection<Subject> _subjectCollection;
        public ObservableCollection<Subject> subjectCollection
        {
            get { return _subjectCollection; }
            set
            {
                _subjectCollection = value;
                RaisePropertyChanged("subjectCollection");
            }
        }

        private ObservableCollection<FavoritesItem> _favotitesCollection;
        public ObservableCollection<FavoritesItem> favotitesCollection
        {
            get { return _favotitesCollection; }
            set
            {
                _favotitesCollection = value;
                RaisePropertyChanged("favotitesCollection");
            }
        }

        public ObservableCollection<Rating> _ratingOfStudentCollection;
        public ObservableCollection<Rating> ratingOfStudentCollection
        {
            get { return _ratingOfStudentCollection; }
            set
            {
                _ratingOfStudentCollection = value;
                RaisePropertyChanged("ratingOfStudentCollection");
            }
        }

        private RatingDatabase rating;
        private RequestManager request;
        private StudentRat _studentRating;
        private GroupRat _groupRating;
        public FavoritesItem CurrentFavoritesItem;
        public RequestManipulation ReqInfo;
    

        public RequestDataViewModel()
        {
            rating = new RatingDatabase(App.DbConnectionString);
            request = new RequestManager();
            ratingOfGroupCollection = new ObservableCollection<Rating>();
            ratingOfStudentCollection = new ObservableCollection<Rating>();
            subjectCollection = new ObservableCollection<Subject>();
            CurrentFavoritesItem = new FavoritesItem();

        }
        
        public async Task GetRatingOfGroupFromServer(RequestManipulation requestInfo)
        {
            ReqInfo = requestInfo;
            var _groupRating = await request.GetRatingOfGroup(ReqInfo);
            ObservableCollection<Rating> ratings;
            ReqInfo.GetRatingFromServer(_groupRating, rating, out ratings);
            ratingOfGroupCollection = ratings;
            ratingOfGroupForView = new ObservableCollection<Rating>(ratingOfGroupCollection.Distinct(new ItemsComparer()).ToList());
            //rating.SaveFavoritestoDb(ReqInfo);
        }

        public async Task GetRatingOfStudentFromServer(RequestManipulation requestInfo)
        {
            ReqInfo = requestInfo;
            _studentRating = await request.GetRatingOfStudent(ReqInfo);
            ObservableCollection<Rating> ratings;
            ReqInfo.GetRatingFromServer(_studentRating, rating, out ratings);
            ratingOfStudentCollection = ratings;
            //rating.SaveFavoritestoDb(ReqInfo);
        }

        public void GetRatingFromDb(RequestManipulation requestInfo)
        {
            ReqInfo = requestInfo;
            ObservableCollection<Rating> groupRatings;
            ObservableCollection<Rating> studentRatings;
            ReqInfo.LoadRatingFromDb(rating, out groupRatings, out studentRatings);
            if (groupRatings != null) ratingOfGroupCollection = groupRatings;
            if (studentRatings != null) ratingOfStudentCollection = studentRatings;
            ratingOfGroupForView = new ObservableCollection<Rating>(ratingOfGroupCollection.Distinct(new ItemsComparer()).ToList());
        }

        internal async Task GetRatingOfStudent(int selectedIndex)
        {
            if (CurrentFavoritesItem != null && (CurrentFavoritesItem.Student != null &&
                                                 CurrentFavoritesItem.Student.Id ==
                                                 ratingOfGroupCollection[selectedIndex].Student.Id))
            {
                GetRatingFromDb(new RequestByStudent(CurrentFavoritesItem));
            }
            else
            {
                var reqInfo = new RequestByStudent()
                {
                    FacultId = CurrentFavoritesItem.Group.FacultId,
                    GroupId = CurrentFavoritesItem.GroupId,
                    Semestr = CurrentFavoritesItem.Semestr,
                    StudentId = ratingOfGroupCollection[selectedIndex].Student.Id
                };

                GetRatingFromDb(reqInfo);
                await GetRatingOfStudentFromServer(reqInfo);
            }
               
        }

        internal void GetFavoriteItem(string ItemId)
        {
            int id = Convert.ToInt32(ItemId);
            CurrentFavoritesItem = rating.GetFavoritesItem(id);
        }


        public void CreateRequest(string facultId, string groupId, string semestr, string studentId)
        {
            if (!string.IsNullOrEmpty(studentId))
                ReqInfo = new RequestByStudent()
                {
                    FacultId = facultId,
                    GroupId = groupId,
                    Semestr = semestr,
                    StudentId = studentId
                };
            else
                ReqInfo = new RequestByGroup()
                {
                    FacultId = facultId,
                    GroupId = groupId,
                    Semestr = semestr
                };
        }
    }
}
