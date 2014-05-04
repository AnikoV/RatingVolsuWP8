using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using RatingVolsuAPI;
using RatingVolsuAPI.Base;

namespace ForTesting
{
    class RequestDataViewModel : PropertyChangedBase
    {
        private ObservableCollection<Rating> _ratingCollection;
        public ObservableCollection<Rating> ratingCollection
        {
            get { return _ratingCollection; }
            set
            {
                _ratingCollection = value;
                RaisePropertyChanged("ratingCollection");
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

        private RatingDatabase rating;
        private RequestManager request;
        private StudentRat _studentRating;
        private GroupRat _groupRating;

        public RequestDataViewModel()
        {
            rating = new RatingDatabase(App.DbConnectionString);
            request = new RequestManager();
            ratingCollection = new ObservableCollection<Rating>();
            subjectCollection = new ObservableCollection<Subject>();
        }

        public async Task GetRatingOfGroup()
        {
            _groupRating = await request.GetRatingOfGroup(RequestInfo.FacultId, RequestInfo.GroupId, RequestInfo.Semestr);

            foreach (var basePredmet in _groupRating.Predmet)
            {
                subjectCollection.Add(new Subject()
                {
                    Id = basePredmet.Key,
                    Name = basePredmet.Value.Name,
                });
            }
            App.ViewModel.studentCollection = new ObservableCollection<Student>();
            foreach (var student in _groupRating.Table)
            {
                App.ViewModel.studentCollection.Add(new Student()
                {
                    Id = student.Key,
                    Number = student.Value.Name
                });
            }


            foreach (var tableItem in _groupRating.Table)
            {
                var stId = tableItem.Key;
                foreach (var predmetItem in tableItem.Value.Predmet)
                {
                    ratingCollection.Add(new Rating()
                    {
                        StudentId = stId,
                        SubjectId = predmetItem.Key,
                        Total = predmetItem.Value,
                        Semestr = RequestInfo.Semestr,
                        Type = _groupRating.Predmet[predmetItem.Key].Type
                    });
                }
            }
            rating.SaveFavoritestoDb(App.ViewModel.facultCollection, App.ViewModel.groupCollection, App.ViewModel.studentCollection,
                                     subjectCollection, ratingCollection);
            GetFavoritesRating();
        }

        public async Task GetRatingOfStudent()
        {
            _studentRating = await request.GetRatingOfStudent(RequestInfo.FacultId, RequestInfo.GroupId, RequestInfo.Semestr, RequestInfo.StudentId);

            foreach (var basePredmet in _studentRating.Predmet)
            {
                subjectCollection.Add(new Subject()
                {
                    Id = basePredmet.Key,
                    Name = basePredmet.Value.Name,
                    Type = basePredmet.Value.Type
                });
            }

            int subjectItem = 0;
            foreach (var item in _studentRating.Table)
            {
                ratingCollection.Add(new Rating()
                {
                    StudentId = RequestInfo.StudentId,
                    SubjectId = subjectCollection[subjectItem].Id,
                    Type = subjectCollection[subjectItem++].Type,
                    Semestr = RequestInfo.Semestr,
                    Att1 = item.Value[0],
                    Att2 = item.Value[1],
                    Att3 = item.Value[2],
                    Sum = item.Value[3],
                    Exam = item.Value[4],
                    Total = item.Value[5]
                });
            }
            rating.SaveFavoritestoDb(App.ViewModel.facultCollection, App.ViewModel.groupCollection, App.ViewModel.studentCollection,
                                     subjectCollection, ratingCollection);
            GetFavoritesRating();
        }
        
        public void GetFavoritesRating()
        {
            var favoritesList = from FavoritesItem item in rating.Favorites
                                                        select item;
            favotitesCollection = new ObservableCollection<FavoritesItem>(favoritesList);
        }
        
    }
}
