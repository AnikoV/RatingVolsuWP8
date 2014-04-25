using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RatingVolsuAPI;

namespace RatingVolsuWP8
{
    class RequestDataViewModel: PropertyChangedBase
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

        public string FacultId;
        public string GroupId;
        public string StudentId;
        public string Semestr;
        private RatingDatabase rating;
        private RequestManager request;
        private StudentRat RatingStudent;
        

        public RequestDataViewModel()
        {
            rating = new RatingDatabase(App.DbConnectionString);
            request = new RequestManager();
        }

        public async Task GetRatingOfStudent(string facultId, string groupId, string semestr, string studentId)
        {
            FacultId = facultId;
            GroupId = groupId;
            StudentId = studentId;
            Semestr = semestr;
            RatingStudent = new StudentRat();
            RatingStudent =  await request.GetRatingOfStudent(facultId, groupId, semestr, studentId);

            ratingCollection = new ObservableCollection<Rating>();
            foreach (var item in RatingStudent.Table)
            {
                ratingCollection.Add(new Rating()
                {
                    Att1 = item.Value[0],
                    Att2 = item.Value[1],
                    Att3 = item.Value[2],
                    Sum = item.Value[3],
                    Exam = item.Value[4],
                    Total = item.Value[5]
                });

            }

            subjectCollection = new ObservableCollection<Subject>();
            foreach (var item in RatingStudent.Predmet)
            {
                subjectCollection.Add(new Subject()
                {
                    Id = item.Key,
                    Name = item.Value
                });

            }


        }

        public void SaveChangesRatingtoDb(string favoritesId)
        {
            
            var currentFavorites = rating.Favorites.FirstOrDefault(x => x.Id == favoritesId);
            if (currentFavorites == null)
            {
                App.ViewModel.SaveChangesRatingtoDb(App.CurrentFavorites);

                for (int i = 0; i < ratingCollection.Count; i++)
                {
                    var ratingItem = ratingCollection[i];
                    ratingItem.StudentId = StudentId;
                    ratingItem.Semestr = Semestr;
                    ratingItem.StudentId = subjectCollection[i].Id;

                    if (rating.Rating.FirstOrDefault(x => x.Id == ratingItem.Id) == null)
                        rating.Rating.InsertOnSubmit(ratingItem);
                }

                foreach (var subject in subjectCollection)
                {
                    subject. = FacultId;
                    if (rating.Groups.FirstOrDefault(x => x.Id == group.Id) == null)
                    {
                        rating.Groups.InsertOnSubmit(group);
                    }

                }
                rating.SubmitChanges();
            }
        }
    }
}
