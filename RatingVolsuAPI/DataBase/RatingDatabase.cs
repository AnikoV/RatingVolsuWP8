using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;

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


        public void SaveFavoritestoDb(RequestInfo requestInfo)
        {
            string ItemName = "";
            if (!String.IsNullOrEmpty(requestInfo.StudentId))
            {
                var orDefault = requestInfo.StudentCollection.FirstOrDefault(x => x.Id == requestInfo.StudentId);
                if (orDefault != null)
                    ItemName = "Студент " + orDefault.Number;
                requestInfo.CurrentRatingType = RatingType.RatingOfStudent;
            }
            else
            {
                var orDefault = requestInfo.GroupCollection.FirstOrDefault(x => x.Id == requestInfo.GroupId);
                if (orDefault != null)
                    ItemName = "Группа " + orDefault.Name;
                requestInfo.CurrentRatingType = RatingType.RatingOfGroup;
            }

            var item = new FavoritesItem()
            {
                StudentId = requestInfo.StudentId,
                GroupId = requestInfo.GroupId,
                Name = ItemName,
                Type = requestInfo.CurrentRatingType,
                Semestr = requestInfo.Semestr

            };
            Favorites.InsertOnSubmit(item);

            foreach (var facult in requestInfo.FacultCollection)
            {
                if (Facults.FirstOrDefault(x => x.Id == facult.Id) == null)
                    Facults.InsertOnSubmit(facult);
            }

            foreach (var group in requestInfo.GroupCollection)
            {
                group.FacultId = requestInfo.FacultId;
                if (Groups.FirstOrDefault(x => x.Id == group.Id) == null)
                {
                    Groups.InsertOnSubmit(group);
                }

            }
            if (requestInfo.StudentCollection != null)
                foreach (var student in requestInfo.StudentCollection)
                {
                    student.GroupId = requestInfo.GroupId;
                    if (Students.FirstOrDefault(x => x.Id == student.Id) == null)
                        Students.InsertOnSubmit(student);
                }
              
            for (int i = 0; i < requestInfo.RatingCollection.Count; i++)
            {
                var ratingItem =requestInfo.RatingCollection[i];
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
                
            foreach (var subject in requestInfo.SubjectCollection)
            {
                if (Subjects.FirstOrDefault(x => x.Id == subject.Id) == null)
                {
                    Subjects.InsertOnSubmit(subject);
                }
            }
            SubmitChanges();
            
        }

        public ObservableCollection<FavoritesItem> GetFavorites()
        {
            var favoritesList = from FavoritesItem item in Favorites
                                select item;
            var favorites = new ObservableCollection<FavoritesItem>(favoritesList);
            return favorites;
        }

        public FavoritesItem GetFavoritesItem(int itemId)
        {
            var favoritesItem = (from FavoritesItem item in Favorites
                                select item).FirstOrDefault(x => x.Id == itemId);
            return favoritesItem;
        }

        public ObservableCollection<Facult> LoadFacults()
        {
            var facults = from Facult item in Facults
                select item;
            return new ObservableCollection<Facult>(facults);
        }

        public ObservableCollection<Group> LoadGroups(string id)
        {
            var groups = from @group in Groups
                where @group.FacultId == id
                select @group;
            return new ObservableCollection<Group>(groups);
        }

        public ObservableCollection<Student> LoadStudents(string id)
        {
            var students = from @student in Students
                where @student.GroupId == id
                select @student;
            return new ObservableCollection<Student>(students);
        } 

        public ObservableCollection<Subject> LoadSubjects( ) 
        {
            //IQueryable<Subject> subjects;
            //if (favotites.Type == RatingType.RatingOfStudent)
            //{
            //        subjects = from Rating rat in Rating
            //        where rat.StudentId == favotites.StudentId && favotites.Semestr == rat.Semestr
            //        select rat.Subject;
            //}
            //else
            //{
            //        subjects = from Rating rat in Rating
            //        where favotites.GroupId == rat.Student.GroupId && favotites.Semestr == rat.Semestr
            //        select rat.Subject;
            //}
            //var subjectCollection = new ObservableCollection<Subject>(subjects.Distinct());
            //return subjectCollection;
        }

        public ObservableCollection<Rating> GetRatingOfGroup(FavoritesItem favotites)
        {
            var rating = from Rating rat in Rating
                         where rat.Student.GroupId == favotites.GroupId && rat.Semestr == favotites.Semestr
                         select rat;
            var ratingCollection = new ObservableCollection<Rating>(rating);
            return ratingCollection;
        }

        public ObservableCollection<Rating> GetRatingOfStudent(FavoritesItem favotites)
        {
            var rating = from Rating rat in Rating
                where rat.StudentId == favotites.StudentId && rat.Semestr == favotites.Semestr
                select rat;
            
            var ratingCollection = new ObservableCollection<Rating>(rating);
            return ratingCollection;
        }
    }
}
