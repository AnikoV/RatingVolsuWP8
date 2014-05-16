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

        private readonly RatingDatabase _rating;
        private readonly RequestManager _request;
        public FavoritesItem CurrentFavoritesItem;
        public RequestManipulation ReqInfo;
    

        public RequestDataViewModel()
        {
            _rating = new RatingDatabase(App.DbConnectionString);
            _request = new RequestManager();
            ratingOfGroupCollection = new ObservableCollection<Rating>();
            ratingOfStudentCollection = new ObservableCollection<Rating>();
            subjectCollection = new ObservableCollection<Subject>();
            CurrentFavoritesItem = new FavoritesItem();

        }
        
        public async Task GetRatingOfGroupFromServer(RequestManipulation requestInfo)
        {
            ReqInfo = requestInfo;
            ratingOfGroupCollection = await _request.GetRatingOfGroup(ReqInfo);
            ratingOfGroupForView = new ObservableCollection<Rating>(ratingOfGroupCollection.Distinct(new ItemsComparer()).ToList());
            subjectCollection = new ObservableCollection<Subject>(ratingOfGroupCollection.Select(x => x.Subject).Distinct(new SubjectsComparer()));
        }

        public async Task GetRatingOfStudentFromServer(RequestManipulation requestInfo)
        {
            ReqInfo = requestInfo;
            ratingOfStudentCollection = await _request.GetRatingOfStudent(ReqInfo);
        }

        public void GetRatingFromDb(RequestManipulation requestInfo)
        {
            ReqInfo = requestInfo;
            ObservableCollection<Rating> groupRatings;
            ObservableCollection<Rating> studentRatings;
            ReqInfo.LoadRatingFromDb(out groupRatings, out studentRatings);
            if (groupRatings != null) ratingOfGroupCollection = groupRatings;
            if (studentRatings != null) ratingOfStudentCollection = studentRatings;
            ratingOfGroupForView = new ObservableCollection<Rating>(ratingOfGroupCollection.Distinct(new ItemsComparer()).ToList());
        }

        internal async Task GetRatingOfStudent(int selectedIndex)
        {
            var reqInfo = new RequestByStudent()
            {
                FacultId = ReqInfo.FacultId,
                GroupId = ReqInfo.GroupId,
                Semestr = ReqInfo.Semestr,
                StudentId = ratingOfGroupForView[selectedIndex].Student.Id
            };

            GetRatingFromDb(reqInfo);
            await GetRatingOfStudentFromServer(reqInfo);
        }

        internal void GetFavoriteItem(string itemId)
        {
            int id = Convert.ToInt32(itemId);
            CurrentFavoritesItem = _rating.GetFavoritesItem(id);
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

        internal void GetRatingBySubject(int selectedIndex)
        {
            ratingOfGroupForView = new ObservableCollection<Rating>(ratingOfGroupCollection
                                                                        .Where(x => x.SubjectId == subjectCollection[selectedIndex].Id).ToList());
        }

        internal bool SaveFavorites()
        {
            var favorites = ReqInfo.GetFavorites();
            return _rating.SaveFavorites(favorites);
        }
    }
}
