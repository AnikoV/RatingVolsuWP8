using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;
using RatingVolsuAPI;

namespace RatingVolsuWP8
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

        public string FacultId;
        public string GroupId;
        public string StudentId;
        public string Semestr;
        private RatingDatabase rating;
        private RequestManager request;
        private StudentRat _studentRating;
        private GroupRat _groupRating;
        private ObservableCollection<Rating> rt;


        public RequestDataViewModel()
        {
            rating = new RatingDatabase(App.DbConnectionString);
            request = new RequestManager();
            rt = new ObservableCollection<Rating>();
            subjectCollection = new ObservableCollection<Subject>();
        }

        public async Task GetRatingOfGroup(string facultId, string groupId, string semestr)
        {
            FacultId = facultId;
            GroupId = groupId;
            Semestr = semestr;
            _groupRating = await request.GetRatingOfGroup(facultId, groupId, semestr);

            foreach (var basePredmet in _groupRating.Predmet)
            {
                subjectCollection.Add(new Subject()
                {
                    Id = basePredmet.Key,
                    Name = basePredmet.Value.Name,
                });
            }
            
            foreach (var tableItem in _groupRating.Table)
            {
                foreach (var predmetItem in tableItem.Value.Predmet)
                {
                    rt.Add(new Rating()
                    {
                        StudentId = tableItem.Key,
                        SubjectId = predmetItem.Key,
                        Total = predmetItem.Value,
                        Semestr = Semestr,
                        Type = _groupRating.Predmet[predmetItem.Key].Type
                    });
                }
            }
            SaveChangesRatingtoDb(App.CurrentFavorites);
        }

        public async Task GetRatingOfStudent(string facultId, string groupId, string semestr, string studentId)
        {
            FacultId = facultId;
            GroupId = groupId;
            StudentId = studentId;
            Semestr = semestr;
            _studentRating = await request.GetRatingOfStudent(facultId, groupId, semestr, studentId);

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
                rt.Add(new Rating()
                {
                    StudentId = StudentId,
                    SubjectId = subjectCollection[subjectItem].Id,
                    Type = subjectCollection[subjectItem++].Type,
                    Semestr = Semestr,
                    Att1 = item.Value[0],
                    Att2 = item.Value[1],
                    Att3 = item.Value[2],
                    Sum = item.Value[3],
                    Exam = item.Value[4],
                    Total = item.Value[5]
                });
            }
            //SaveChangesRatingtoDb(App.CurrentFavorites);
        }

        public void SaveChangesRatingtoDb(int favoritesId)
        {
            var currentFavorites = rating.Favorites.FirstOrDefault(x => x.Id == favoritesId);
            string ItemName = "";
            if (currentFavorites == null)
            {
                if (App.CacheManager.CurrentRatingType == RatingType.RatingOfStudent)
                {
                    var orDefault = App.CacheManager.studentCollection.FirstOrDefault(x => x.Id == StudentId);
                    if (orDefault != null)
                        ItemName = "Студент " + orDefault.Number;
                }
                else
                {
                    var orDefault = App.CacheManager.groupCollection.FirstOrDefault(x => x.Id == GroupId);
                    if (orDefault != null)
                        ItemName = "Группа " + orDefault.Name;
                }

                var item = new FavoritesItem()
                {
                    StudentId = StudentId,
                    GroupId = GroupId,
                    Name = ItemName,
                    Type = App.CacheManager.CurrentRatingType

                };
                rating.Favorites.InsertOnSubmit(item);

                foreach (var facult in App.CacheManager.facultCollection)
                {
                    if (rating.Facults.FirstOrDefault(x => x.Id == facult.Id) == null)
                        rating.Facults.InsertOnSubmit(facult);
                }

                foreach (var group in App.CacheManager.groupCollection)
                {
                    group.FacultId = FacultId;
                    if (rating.Groups.FirstOrDefault(x => x.Id == group.Id) == null)
                    {
                        rating.Groups.InsertOnSubmit(group);
                    }

                }
                if (App.CacheManager.studentCollection != null)
                    foreach (var student in App.CacheManager.studentCollection)
                    {
                        student.GroupId = GroupId;
                        if (rating.Students.FirstOrDefault(x => x.Id == student.Id) == null)
                            rating.Students.InsertOnSubmit(student);
                    }

                for (int i = 0; i < rt.Count; i++)
                {
                    var ratingItem = rt[i];

                    if (rating.Rating.FirstOrDefault(x => x.StudentId == ratingItem.StudentId &&
                                                          x.SubjectId == ratingItem.SubjectId) == null)
                        rating.Rating.InsertOnSubmit(ratingItem);
                }

                foreach (var subject in subjectCollection)
                {
                    if (rating.Subjects.FirstOrDefault(x => x.Id == subject.Id) == null)
                    {
                        rating.Subjects.InsertOnSubmit(subject);
                    }
                }
                rating.SubmitChanges();
                GetFavoritesRating();
            }
        }

        public void GetFavoritesRating()
        {
            var favoritesList = from FavoritesItem item in rating.Favorites
                                                        select item;
            favotitesCollection = new ObservableCollection<FavoritesItem>(favoritesList);
        }
        
    }
}
