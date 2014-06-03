using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatingVolsuAPI;

namespace RatingVolsuAPI
{
    public abstract class RequestManipulation
    {
        public string FacultId;
        public string GroupId;
        public string Semestr;
        public RatingDatabase rating = new RatingDatabase(Info.DbConnectionString);

        public abstract ObservableCollection<Rating> GetRatingFromServer(Object Rating);

        public abstract string GetParams();

        public abstract ObservableCollection<Rating> LoadRatingFromDb();

        public abstract FavoritesItem GetFavorites();
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

        public override ObservableCollection<Rating> GetRatingFromServer(Object RatingObject)
        {
            var groupRating = (GroupRat)RatingObject;
            foreach (var subject in groupRating.Predmet)
            {
                if (rating.Subjects.FirstOrDefault(x => x.Id == subject.Key) == null)
                {
                    rating.Subjects.InsertOnSubmit(new Subject()
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
                if (rating.Students.FirstOrDefault(x => x.Id == tableItem.Key) == null)
                    rating.Students.InsertOnSubmit(
                        new Student()
                        {
                            Id = tableItem.Key,
                            GroupId = GroupId,
                            Number = tableItem.Value.Name
                        });
                foreach (var predmetItem in tableItem.Value.Predmet)
                {
                    var ratingitemFromDb =
                        (from Rating itemFromDb in rating.Rating
                            where itemFromDb.StudentId == tableItem.Key &&
                                    itemFromDb.SubjectId == predmetItem.Key &&
                                    itemFromDb.Semestr == Semestr
                            select itemFromDb).FirstOrDefault();
                    if (ratingitemFromDb == null)
                        rating.Rating.InsertOnSubmit(new Rating()
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
            rating.SubmitChanges();
            return rating.GetRatingOfGroup(this);
        }

        public override ObservableCollection<Rating> LoadRatingFromDb()
        {
            return rating.GetRatingOfGroup(this);
        }

        public override string GetParams()
        {
            string s = "Fak=" + FacultId +
                       "&Group=" + GroupId +
                       "&Semestr=" + Semestr;
            return s;
        }

        public override FavoritesItem GetFavorites()
        {
            return new FavoritesItem()
            {
                GroupId = GroupId,
                Semestr = Semestr,
                Type = RatingType.RatingOfGroup
            };
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

        public override FavoritesItem GetFavorites()
        {
            return new FavoritesItem()
            {
                GroupId = GroupId,
                Semestr = Semestr,
                Type = RatingType.RatingOfStudent,
                StudentId = StudentId
            };
        }

        public override ObservableCollection<Rating> GetRatingFromServer(object ratingObject)
        {
            var studentRating = (StudentRat) ratingObject;

            foreach (var subject in studentRating.Predmet)
            {
                if (rating.Subjects.FirstOrDefault(x => x.Id == subject.Key) == null)
                {
                    rating.Subjects.InsertOnSubmit(new Subject()
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
                    (from Rating itemFromDb in rating.Rating
                     where itemFromDb.StudentId == StudentId &&
                             itemFromDb.SubjectId == ratingItem.Key &&
                             itemFromDb.Semestr == Semestr
                     select itemFromDb).FirstOrDefault();
                if (ratingitemFromDb == null)
                    rating.Rating.InsertOnSubmit(new Rating()
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
            rating.SubmitChanges();
            return rating.GetRatingOfStudent(this);
        }

        public override ObservableCollection<Rating> LoadRatingFromDb()
        {
            return rating.GetRatingOfStudent(this);
        }

    }
}
