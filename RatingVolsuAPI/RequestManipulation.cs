using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatingVolsuAPI;

namespace RatinVolsuAPI
{
    public abstract class RequestManipulation
    {
        public string FacultId;
        public string GroupId;
        public string Semestr;

        public abstract void GetRatingFromServer(Object Rating, out ObservableCollection<Subject> subjectCollection,
            out ObservableCollection<Rating> ratingOfGroupCollection);

        public abstract string GetParams();

        public abstract void LoadRatingFromDb(RatingDatabase rating, out ObservableCollection<Rating> groupRatings,
            out ObservableCollection<Rating> studentRatings);

    }

    public class RequestByGroup : RequestManipulation
    {
        public RequestByGroup() { }
        public RequestByGroup(FavoritesItem favorites)
        {
            FacultId = favorites.Student.Group.FacultId;
            GroupId = favorites.Student.GroupId;
            Semestr = favorites.Semestr;
        }

        public override void GetRatingFromServer(Object rating, out ObservableCollection<Subject> subjectCollection, out ObservableCollection<Rating> ratingOfGroupCollection)
        {
            ratingOfGroupCollection = new ObservableCollection<Rating>();
            subjectCollection = new ObservableCollection<Subject>();
            var groupRating = (GroupRat)rating;
            foreach (var basePredmet in groupRating.Predmet)
            {
                subjectCollection.Add(new Subject()
                {
                    Id = basePredmet.Key,
                    Name = basePredmet.Value.Name,
                    Type = basePredmet.Value.Type
                });
            }
            var studentCollection = new ObservableCollection<Student>();
            foreach (var student in groupRating.Table)
            {
                studentCollection.Add(new Student()
                {
                    Id = student.Key,
                    Number = student.Value.Name
                });
            }

            foreach (var tableItem in groupRating.Table)
            {
                var stId = tableItem.Key;
                foreach (var predmetItem in tableItem.Value.Predmet)
                {
                    ratingOfGroupCollection.Add(new Rating()
                    {
                        StudentId = stId,
                        SubjectId = predmetItem.Key,
                        Total = predmetItem.Value,
                        Semestr = Semestr,
                    });
                }
            }
        }

        public override void LoadRatingFromDb(RatingDatabase rating, out ObservableCollection<Rating> groupRatings, out ObservableCollection<Rating> studentRatings)
        {
            groupRatings = rating.GetRatingOfGroup(this);
            studentRatings = null;
        }

        public override string GetParams()
        {
            string s = "facult=" + FacultId +
                       "&group=" + GroupId +
                       "&semestr=" + Semestr;
            return s;
        }
    }

    public class RequestByStudent : RequestManipulation
    {
        public string StudentId;
 
        public RequestByStudent(Student student, string semestr)
        {
            FacultId = student.Group.FacultId;
            GroupId = student.GroupId;
            Semestr = semestr;
            StudentId = student.Id;
        }

        public RequestByStudent(FavoritesItem favorites)
        {
            FacultId = favorites.Student.Group.FacultId;
            GroupId = favorites.Student.GroupId;
            Semestr = favorites.Semestr;
            StudentId = favorites.Student.Id;
        }

        public RequestByStudent()
        {
            // TODO: Complete member initialization
        }

        public override string GetParams()
        {
            string s = "facult=" + FacultId +
                       "&group=" + GroupId +
                       "&semestr=" + Semestr +
                       "student=" + StudentId;
            return s;
        }

        public override void GetRatingFromServer(object rating, out ObservableCollection<Subject> subjectCollection,
            out ObservableCollection<Rating> ratingOfGroupCollection)
        {
            var studentRating = (StudentRat) rating;
            subjectCollection = new ObservableCollection<Subject>();
            ratingOfGroupCollection = new ObservableCollection<Rating>();
            foreach (var basePredmet in studentRating.Predmet)
            {
                subjectCollection.Add(new Subject()
                {
                    Id = basePredmet.Key,
                    Name = basePredmet.Value.Name,
                    Type = basePredmet.Value.Type
                });
            }

            int subjectItem = 0;
            foreach (var item in studentRating.Table)
            {
                ratingOfGroupCollection.Add(new Rating()
                {
                    StudentId = StudentId,
                    SubjectId = subjectCollection[subjectItem++].Id,
                    Semestr = Semestr,
                    Att1 = item.Value[0],
                    Att2 = item.Value[1],
                    Att3 = item.Value[2],
                    Sum = item.Value[3],
                    Exam = item.Value[4],
                    Total = item.Value[5]
                });
            }
        }

        public override void LoadRatingFromDb(RatingDatabase rating, out ObservableCollection<Rating> groupRatings, out ObservableCollection<Rating> studentRatings)
        {
            studentRatings = rating.GetRatingOfStudent(this);
            groupRatings = null;
        }


    }
}
