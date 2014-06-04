using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using RatingVolsuAPI;
using RatingVolsuAPI.DataBase;

namespace RatingVolsuAPI
{
    public class RatingDatabase:DataContext
    {
        public RatingDatabase(string connectionString)
            : base(connectionString)
        { }

        public Table<Facult> Facults;

        public Table<Group> Groups;

        public Table<Semestr> SemestrItems;

        public Table<Student> Students;

        public Table<Subject> Subjects;

        public Table<Rating> Rating;

        public Table<FavoritesItem> Favorites;

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

        public ObservableCollection<Rating> GetRatingOfGroup(RequestByGroup rm)
        {
            var rating = from Rating rat in Rating
                         where rat.Student.GroupId == rm.GroupId && rat.Semestr == rm.Semestr
                         select rat;
            var ratingCollection = new ObservableCollection<Rating>(rating);
            return ratingCollection;
        }

        public ObservableCollection<Rating> GetRatingOfStudent(RequestByStudent req)
        {
            var rating = from Rating rat in Rating
                where rat.StudentId == req.StudentId && rat.Semestr == req.Semestr
                select rat;
            
            var ratingCollection = new ObservableCollection<Rating>(rating);
            return ratingCollection;
        }

        public bool CheckFavorites(RequestManipulation request)
        {
            FavoritesItem favoritesItem;
            if (request.GetType() == typeof(RequestByGroup))
                favoritesItem = (from FavoritesItem item in Favorites
                    select item).FirstOrDefault(x => x.GroupId == request.GroupId &&
                                                     x.Semestr == request.Semestr);
            else
            {
                var req = (RequestByStudent) request;
                favoritesItem = (from FavoritesItem item in Favorites
                    select item).FirstOrDefault(x => x.StudentId == req.StudentId &&
                                                     x.Semestr == req.Semestr);
            }
            if (favoritesItem != null)
                return false;
            return true;
        }
        public void SaveFavorites(RequestManipulation request, string name)
        {
            var favorites = request.GetFavorites(name);
            Favorites.InsertOnSubmit(favorites);
            SubmitChanges();
        }

        public void DeleteFavorites(FavoritesItem favoritesItem)
        {
            Favorites.DeleteOnSubmit(favoritesItem);
            SubmitChanges();
        }
    }
}
