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
        public RatingDatabase rating = new RatingDatabase();

        public abstract ObservableCollection<Rating> GetRatingFromServer(Object Rating);

        public abstract string GetParams();

        public abstract ObservableCollection<Rating> LoadRatingFromDb();

        public abstract FavoritesItem GetFavorites(string name);
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
            if (groupRating.Predmet != null)
            {
                rating.Subjects.InsertEntity(groupRating.Predmet.ToSubjectCollection());
                rating.SubmitChanges();
            }
            if (groupRating.Table != null)
            {
                rating.Students.InsertEntity(groupRating.Table.ToStudentCollection(GroupId));

                foreach (var tableItem in groupRating.Table)
                    rating.Rating.InsertEntity(tableItem.Value.Predmet.ToRatingCollection(tableItem.Key, Semestr));
                rating.SubmitChanges();
            }

            var result = rating.GetRatingOfGroup(this);
            return (result.Count == 0) ? null : result;
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

        public override FavoritesItem GetFavorites(string name)
        {
            return new FavoritesItem()
            {
                Id = GroupId + Semestr,
                Name = name,
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

        public override FavoritesItem GetFavorites(string name)
        {
            return new FavoritesItem()
            {
                Id = StudentId + Semestr,
                Name = name,
                Semestr = Semestr,
                Type = RatingType.RatingOfStudent,
                StudentId = StudentId
            };
        }

        public override ObservableCollection<Rating> GetRatingFromServer(object ratingObject)
        {
            var studentRating = (StudentRat) ratingObject;

            if (studentRating.Predmet != null)
            {
                var subjects = studentRating.Predmet.ToSubjectCollection();
                rating.Subjects.InsertEntity(subjects);
                rating.SubmitChanges();
            }
            if (studentRating.Table != null)
            {
                var ratings = studentRating.Table.ToRatingCollection(StudentId, Semestr);
                rating.Rating.InsertEntity(ratings);
                rating.SubmitChanges();
            }
           
            return rating.GetRatingOfStudent(this);
        }

        public override ObservableCollection<Rating> LoadRatingFromDb()
        {
            return rating.GetRatingOfStudent(this);
        }

    }
}
