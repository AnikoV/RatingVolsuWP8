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
                    Total = total,
                    Semestr = semestr
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
