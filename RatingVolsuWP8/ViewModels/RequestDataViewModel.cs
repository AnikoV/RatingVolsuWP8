using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private StudentRat RatingStudent;
        private ObservableCollection<Rating> rt;


        public RequestDataViewModel()
        {
            rating = new RatingDatabase(App.DbConnectionString);
            request = new RequestManager();
            rt = new ObservableCollection<Rating>();
            subjectCollection = new ObservableCollection<Subject>();
        }

        public async Task GetRatingOfStudent(string facultId, string groupId, string semestr, string studentId)
        {
            FacultId = facultId;
            GroupId = groupId;
            StudentId = studentId;
            Semestr = semestr;
            RatingStudent = await request.GetRatingOfStudent(facultId, groupId, semestr, studentId);
            
            foreach (var item in RatingStudent.Table)
            {
                rt.Add(new Rating()
                {
                    Att1 = item.Value[0],
                    Att2 = item.Value[1],
                    Att3 = item.Value[2],
                    Sum = item.Value[3],
                    Exam = item.Value[4],
                    Total = item.Value[5]
                });

            }

            foreach (var item in RatingStudent.Predmet)
            {
                subjectCollection.Add(new Subject()
                {
                    Id = item.Key,
                    Name = item.Value
                });

            }
            SaveChangesRatingtoDb(App.CurrentFavorites);
        }

        public void SaveChangesRatingtoDb(int favoritesId)
        {

            var currentFavorites = rating.Favorites.FirstOrDefault(x => x.Id == favoritesId);
            string ItemName = "";
            if (currentFavorites == null)
            {
                Debug.Assert(App.CacheManager.studentCollection != null, "studentCollection != null");
                var orDefault = App.CacheManager.studentCollection.FirstOrDefault(x => x.Id == StudentId);
                if (orDefault != null)
                {
                    ItemName = "Студент " + orDefault.Number;
                    var item = new FavoritesItem()
                    {
                        StudentId = StudentId,
                        Name = ItemName,
                        Type = RatingType.RatingOfStudent

                    };
                    rating.Favorites.InsertOnSubmit(item);
                }

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
                foreach (var student in App.CacheManager.studentCollection)
                {
                    student.GroupId = GroupId;
                    if (rating.Students.FirstOrDefault(x => x.Id == student.Id) == null)
                        rating.Students.InsertOnSubmit(student);
                }

                for (int i = 0; i < rt.Count; i++)
                {
                    var ratingItem = rt[i];
                    ratingItem.StudentId = StudentId;
                    ratingItem.Semestr = Semestr;
                    ratingItem.SubjectId = subjectCollection[i].Id;

                    if (rating.Rating.FirstOrDefault(x => x.Id == ratingItem.Id) == null)
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
