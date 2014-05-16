﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using RatinVolsuAPI;

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

        public bool SaveFavorites(FavoritesItem favorites)
        {
            FavoritesItem favoritesItem;
            if (favorites.Type == RatingType.RatingOfGroup)
                 favoritesItem = (from FavoritesItem item in Favorites
                                 select item).FirstOrDefault(x => x.GroupId == favorites.GroupId &&
                                                                  x.Semestr == favorites.Semestr);
            else
                favoritesItem = (from FavoritesItem item in Favorites
                                 select item).FirstOrDefault(x => x.StudentId == favorites.StudentId &&
                                                                      x.Semestr == favorites.Semestr);
            if (favoritesItem != null)
                return false;
            Favorites.InsertOnSubmit(favorites);
            SubmitChanges();
            return true;
        }

        public void DeleteFavorites(FavoritesItem favoritesItem)
        {
            Favorites.DeleteOnSubmit(favoritesItem);
            SubmitChanges();
        }
    }
}
