using System.Data.Linq;
using System.Linq;

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


    }
}
