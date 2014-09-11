using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatingVolsuAPI
{
    public static class CollectionExtension
    {
        public static ObservableCollection<Rating> AllSubjects(this ObservableCollection<Rating> collection)
        {
            var distinctCollect = new ObservableCollection<Rating>(collection.Distinct(new ItemsComparer()));
            foreach (var rating in distinctCollect)
            {
                collection.Add(new Rating()
                {
                    StudentId = rating.StudentId,
                    Student = rating.Student,
                    Total = collection.Where(x => x.StudentId == rating.StudentId).Sum(x => x.Total),
                    SubjectId = "000",
                    Id = rating.StudentId + "000"
                });
            }
            return collection;

        }
    }
}
