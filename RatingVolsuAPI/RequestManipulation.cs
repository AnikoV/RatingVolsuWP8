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

        public abstract void GetRatingFromServer(Object Rating, RatingDatabase db, out ObservableCollection<Rating> ratingOfGroupCollection);

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

        public override void GetRatingFromServer(Object rating, RatingDatabase db,  out ObservableCollection<Rating> ratingOfGroupCollection)
        {
            var groupRating = (GroupRat)rating;
            foreach (var subject in groupRating.Predmet)
            {
                if (db.Subjects.FirstOrDefault(x => x.Id == subject.Key) == null)
                {
                    db.Subjects.InsertOnSubmit(new Subject()
                    {
                        Id = subject.Key,
                        Name = subject.Value.Name,
                        Type = subject.Value.Type
                    });
                }
            }
          
            foreach (var tableItem in groupRating.Table)
            {
                var stId = tableItem.Key;
                if (db.Students.FirstOrDefault(x => x.Id == tableItem.Key) == null)
                    db.Students.InsertOnSubmit(
                        new Student()
                        {
                            Id = tableItem.Key,
                            Number = tableItem.Value.Name
                        });
                foreach (var predmetItem in tableItem.Value.Predmet)
                {
                    var ratingitemFromDb =
                        (from Rating itemFromDb in db.Rating
                            where itemFromDb.StudentId == tableItem.Key &&
                                    itemFromDb.SubjectId == predmetItem.Key &&
                                    itemFromDb.Semestr == Semestr
                            select itemFromDb).FirstOrDefault();
                    if (ratingitemFromDb == null)
                        db.Rating.InsertOnSubmit(new Rating()
                        {
                            StudentId = stId,
                            SubjectId = predmetItem.Key,
                            Total = predmetItem.Value,
                            Semestr = Semestr,
                        });
                    else
                        ratingitemFromDb.Total = predmetItem.Value;
                }
            }
            db.SubmitChanges();
            ratingOfGroupCollection = db.GetRatingOfGroup(this);
        }

        public override void LoadRatingFromDb(RatingDatabase rating, out ObservableCollection<Rating> groupRatings, out ObservableCollection<Rating> studentRatings)
        {
            groupRatings = rating.GetRatingOfGroup(this);
            studentRatings = null;
        }

        public override string GetParams()
        {
            string s = "Fak=" + FacultId +
                       "&Group=" + GroupId +
                       "&Semestr=" + Semestr;
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
            string s = "Fak=" + FacultId +
                       "&Group=" + GroupId +
                       "&Semestr=" + Semestr +
                       "&Zach=" + StudentId;
            return s;
        }

        public override void GetRatingFromServer(object rating, RatingDatabase db, out ObservableCollection<Rating> ratingOfStudentCollection)
        {
            var studentRating = (StudentRat) rating;

            foreach (var subject in studentRating.Predmet)
            {
                if (db.Subjects.FirstOrDefault(x => x.Id == subject.Key) == null)
                {
                    db.Subjects.InsertOnSubmit(new Subject()
                    {
                        Id = subject.Key,
                        Name = subject.Value.Name,
                        Type = subject.Value.Type    
                    });
                }
            }

            foreach (var ratingItem in studentRating.Table)
            {
                var ratingitemFromDb =
                    (from Rating itemFromDb in db.Rating
                     where itemFromDb.StudentId == StudentId &&
                             itemFromDb.SubjectId == ratingItem.Key &&
                             itemFromDb.Semestr == Semestr
                     select itemFromDb).FirstOrDefault();
                if (ratingitemFromDb == null)
                    db.Rating.InsertOnSubmit(new Rating()
                    {
                        StudentId = StudentId,
                        SubjectId = ratingItem.Key,
                        Semestr = Semestr,
                        Att1 = ratingItem.Value[0],
                        Att2 = ratingItem.Value[1],
                        Att3 = ratingItem.Value[2],
                        Sum = ratingItem.Value[3],
                        Exam = ratingItem.Value[4],
                        Total = ratingItem.Value[5]
                    });
                else
                {
                    if (!String.IsNullOrEmpty(ratingItem.Value[0])) ratingitemFromDb.Att1 = ratingItem.Value[0];
                    if (!String.IsNullOrEmpty(ratingItem.Value[1])) ratingitemFromDb.Att2 = ratingItem.Value[1];
                    if (!String.IsNullOrEmpty(ratingItem.Value[2])) ratingitemFromDb.Att3 = ratingItem.Value[2];
                    if (!String.IsNullOrEmpty(ratingItem.Value[3])) ratingitemFromDb.Sum = ratingItem.Value[3];
                    if (!String.IsNullOrEmpty(ratingItem.Value[4])) ratingitemFromDb.Exam = ratingItem.Value[4];
                    if (!String.IsNullOrEmpty(ratingItem.Value[5])) ratingitemFromDb.Total = ratingItem.Value[5];
                }

            }
            db.SubmitChanges();
            ratingOfStudentCollection = db.GetRatingOfStudent(this);
        }

        public override void LoadRatingFromDb(RatingDatabase rating, out ObservableCollection<Rating> groupRatings, out ObservableCollection<Rating> studentRatings)
        {
            studentRatings = rating.GetRatingOfStudent(this);
            groupRatings = null;
        }


    }
}
