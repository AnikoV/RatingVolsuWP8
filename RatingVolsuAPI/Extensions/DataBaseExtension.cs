using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatingVolsuAPI
{
    public static class DataBaseExtension
    {
        public static bool IsNew<T>(this Table<T> entity, T item) where T : class, IRepository
        {
            return entity.FirstOrDefault(x => x.Id.Equals(item.Id)) == null;
        }

        public static ObservableCollection<Subject> ToSubjectCollection(this Dictionary<string, BasePredmet> dictionary)
        {
            var collection = new ObservableCollection<Subject>();
            foreach (var subject in dictionary)
            {
                collection.Add(new Subject()
                {
                    Id = subject.Key,
                    Name = subject.Value.Name,
                    Type = subject.Value.Type
                });

            }
            return collection;
        }

        public static ObservableCollection<Student> ToStudentCollection(this Dictionary<string, BaseStudent> dictionary, string groupId)
        {
            var collection = new ObservableCollection<Student>();
            foreach (var student in dictionary)
            {
                collection.Add(new Student()
                {
                    Id = student.Key,
                    GroupId = groupId,
                    Number = student.Value.Name
                });

            }
            return collection;
        }

        public static ObservableCollection<Rating> ToRatingCollection(this Dictionary<string, string> dictionary, string studentId, string semestr)
        {
            var collection = new ObservableCollection<Rating>();
            foreach (var student in dictionary)
            {
                string total;
                total = student.Value.IndexOf('(') != -1 ? student.Value.Remove(student.Value.IndexOf('(')) : student.Value;
                collection.Add(new Rating()
                {
                    Id = studentId + student.Key + semestr,
                    StudentId = studentId,
                    SubjectId = student.Key,
                    Total = Convert.ToInt32(total),
                    Semestr = semestr
                });

            }
            return collection;
        }

        public static ObservableCollection<Rating> ToRatingCollection(this Dictionary<string, List<string>> dictionary, string studentId, string semestr)
        {
            var collection = new ObservableCollection<Rating>();
            foreach (var predmet in dictionary)
            {
                string total;
                total = predmet.Value[5].IndexOf('(') != -1 ? predmet.Value[5].Remove(predmet.Value[5].IndexOf('(')) : predmet.Value[5];
                Rating newRating = new Rating();
                int parsedValue;
                collection.Add(new Rating()
                {
                    Id = studentId + predmet.Key + semestr,
                    StudentId = studentId,
                    SubjectId = predmet.Key,
                    Semestr = semestr,
                    Att1 = Int32.TryParse(predmet.Value[0], out parsedValue) ? parsedValue : (int?) null,
                    Att2 = Int32.TryParse(predmet.Value[1], out parsedValue) ? parsedValue : (int?)null,
                    Att3 = Int32.TryParse(predmet.Value[2], out parsedValue) ? parsedValue : (int?)null,
                    Sum = Int32.TryParse(predmet.Value[3], out parsedValue) ? parsedValue : (int?)null,
                    Exam = Int32.TryParse(predmet.Value[4], out parsedValue) ? parsedValue : (int?)null,
                    Total = Int32.TryParse(total, out parsedValue) ? parsedValue : (int?)null
                });

            }
            return collection;
        }

        public static void InsertEntity<T>(this Table<T> entity, ObservableCollection<T> collection)
            where T : class, IRepository
        {
            foreach (var item in collection)
            {
                if (entity.IsNew(item))
                    entity.InsertOnSubmit(item);
                else
                {
                    var field = entity.FirstOrDefault(x => x.Id.Equals(item.Id));
                    if (field != null) 
                        field.Update(item);
                }
            }
        }

    }
}
