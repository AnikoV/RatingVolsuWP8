using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatingVolsuAPI;

namespace RatinVolsuAPI
{
    public class RequestManipulation
    {
        public string FacultId;
        public string GroupId;
        public string Semestr;

        public virtual string GetParams()
        {
            string s = "facult=" + FacultId +
                       "&group=" + GroupId +
                       "&semestr=" + Semestr;
            return s;
        }

        public virtual void GetRating(Object _groupRating, out ObservableCollection<Subject> subjectCollection, out ObservableCollection<Rating> ratingOfGroupCollection)
        {
            ratingOfGroupCollection = new ObservableCollection<Rating>();
            subjectCollection = new ObservableCollection<Subject>();
            foreach (var basePredmet in _groupRating.Predmet)
            {
                subjectCollection.Add(new Subject()
                {
                    Id = basePredmet.Key,
                    Name = basePredmet.Value.Name,
                    Type = basePredmet.Value.Type
                });
            }
            var studentCollection = new ObservableCollection<Student>();
            foreach (var student in _groupRating.Table)
            {
                studentCollection.Add(new Student()
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
    }

    public class RequestByGroup : RequestManipulation
    {
        
    }

    public class RequestByStudent : RequestManipulation
    {
        public string StudentId;
        private FavoritesItem CurrentFavoritesItem;

        public RequestByStudent(FavoritesItem CurrentFavoritesItem)
        {
            FacultId = CurrentFavoritesItem.Group.FacultId;
            GroupId = CurrentFavoritesItem.GroupId;
            Semestr = CurrentFavoritesItem.Semestr;
            StudentId = ratingOfGroupCollection[selectedIndex].Student.Id;
        }

        public virtual string GetParams()
        {
            string s = "facult=" + FacultId +
                       "&group=" + GroupId +
                       "&semestr=" + Semestr +
                       "student=" + StudentId;
            return s;
        }

        public virtual void GetRating(object _studRating, out ObservableCollection<Subject> subjectCollection,
            out ObservableCollection<Rating> ratingOfGroupCollection)
        {
            
        }
    }
}
