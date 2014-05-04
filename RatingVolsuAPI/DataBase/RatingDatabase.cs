using System;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace RatingVolsuAPI
{
    public class RatingDatabase:DataContext
    {
        public RatingDatabase(string connectionString)
            : base(connectionString)
        { }

        public Table<Facult> Facults;

        public Table<Group> Groups;

        public Table<Student> Students;

        public Table<Subject> Subjects;

        public Table<Rating> Rating;

        public Table<FavoritesItem> Favorites;

        public void SaveFavoritestoDb(ObservableCollection<Facult> facultCollection, ObservableCollection<Group> groupCollection,
                                      ObservableCollection<Student> studentCollection, ObservableCollection<Subject> subjectCollection,
                                      ObservableCollection<Rating> ratingCollection)
        {
 	        var currentFavorites = Favorites.FirstOrDefault(x => x.Id == RequestInfo.CurrentFavorites);
            string ItemName = "";
            if (currentFavorites == null)
            {
                if (RequestInfo.CurrentRatingType == RatingType.RatingOfStudent)
                {
                    var orDefault = studentCollection.FirstOrDefault(x => x.Id == RequestInfo.StudentId);
                    if (orDefault != null)
                        ItemName = "Студент " + orDefault.Number;
                }
                else
                {
                    var orDefault = groupCollection.FirstOrDefault(x => x.Id == RequestInfo.GroupId);
                    if (orDefault != null)
                        ItemName = "Группа " + orDefault.Name;
                }

                var item = new FavoritesItem()
                {
                    StudentId = RequestInfo.StudentId,
                    GroupId = RequestInfo.GroupId,
                    Name = ItemName,
                    Type = RequestInfo.CurrentRatingType,
                    Semestr = RequestInfo.Semestr

                };
                Favorites.InsertOnSubmit(item);

                foreach (var facult in facultCollection)
                {
                    if (Facults.FirstOrDefault(x => x.Id == facult.Id) == null)
                        Facults.InsertOnSubmit(facult);
                }

                foreach (var group in groupCollection)
                {
                    group.FacultId = RequestInfo.FacultId;
                    if (Groups.FirstOrDefault(x => x.Id == group.Id) == null)
                    {
                        Groups.InsertOnSubmit(group);
                    }

                }
                if (studentCollection != null)
                    foreach (var student in studentCollection)
                    {
                        student.GroupId = RequestInfo.GroupId;
                        if (Students.FirstOrDefault(x => x.Id == student.Id) == null)
                            Students.InsertOnSubmit(student);
                    }
              
                for (int i = 0; i < ratingCollection.Count; i++)
                {
                    var ratingItem = ratingCollection[i];
                    var ratingitemFromDb =
                        (from Rating itemFromDb in Rating
                            where itemFromDb.StudentId == ratingItem.StudentId &&
                                    itemFromDb.SubjectId == ratingItem.SubjectId &&
                                    itemFromDb.Semestr == ratingItem.Semestr
                            select itemFromDb).FirstOrDefault();
                    if (ratingitemFromDb == null)
                        Rating.InsertOnSubmit(ratingItem);
                    else
                    {
                        if (!String.IsNullOrEmpty(ratingItem.Att1)) ratingitemFromDb.Att1 = ratingItem.Att1;
                        if (!String.IsNullOrEmpty(ratingItem.Att2)) ratingitemFromDb.Att2 = ratingItem.Att2;
                        if (!String.IsNullOrEmpty(ratingItem.Att3)) ratingitemFromDb.Att3 = ratingItem.Att3;
                        if (!String.IsNullOrEmpty(ratingItem.Exam)) ratingitemFromDb.Exam = ratingItem.Exam;
                        if (!String.IsNullOrEmpty(ratingItem.Sum)) ratingitemFromDb.Sum = ratingItem.Sum;
                    }

                }
                
                foreach (var subject in subjectCollection)
                {
                    if (Subjects.FirstOrDefault(x => x.Id == subject.Id) == null)
                    {
                        Subjects.InsertOnSubmit(subject);
                    }
                }
                SubmitChanges();
            }
            
        }       
    }
}
