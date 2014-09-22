using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatingVolsuAPI;

namespace RatingVolsuWP8
{
    internal class InputDataViewModel : PropertyChangedBase
    {
        //Settings
        public InputDataMode CurrentInputMode;
        public RatingType CurrentRatingType;
        public RequestManipulation RequestManip;
        private readonly RequestManager _requestManager;
        public FavoritesItem CurrentFavoritesItem;
        private RatingDatabase ratingDb;
        //Collections
        public ObservableCollection<Facult> Facults { get; set; }
        public ObservableCollection<Group> Groups { get; set; }
        public ObservableCollection<Student> Students { get; set; }
        public ObservableCollection<Semester> Semesters { get; set; }

        public InputDataViewModel()
        {
            Facults = new ObservableCollection<Facult>();
            Groups = new ObservableCollection<Group>();
            Students = new ObservableCollection<Student>();
            Semesters = new ObservableCollection<Semester>();
            _requestManager = new RequestManager();
            ratingDb = new RatingDatabase();
        }

        #region Requests

        public async Task GetFacults()
        {
            Facults = await _requestManager.GetFacultList();
        }

        public async Task GetGroups(int selectedIndex)
        {
            RequestManip.FacultId = Facults[selectedIndex].Id;
            Groups = await _requestManager.GetGroupList(RequestManip.FacultId);
        }

        public int GetSemestrCount()
        {
            var firstOrDefault = Groups.FirstOrDefault(x => x.Id == RequestManip.GroupId);
            if (firstOrDefault != null)
            {
                int period = DateTime.Now.Year - Convert.ToInt32(firstOrDefault.Year);
                period *= 2;
                return period;
            }
            return 0;
        }

        public async Task GetStudents()
        {
            Students = await _requestManager.GetStudentList(RequestManip.GroupId);
        }
        #endregion

        public async Task<List<string>> GetSemestrList(string groupId)
        {
            var list = await _requestManager.GetSemestrList(groupId);
            return list;
        }

        internal void GetFavoriteItem(string itemId)
        {
            CurrentFavoritesItem = ratingDb.GetFavoritesItem(itemId);
            CurrentRatingType = CurrentFavoritesItem.Type;
        }

        internal void EditFavorites()
        {
            FavoritesItem favoritesItem = (from FavoritesItem item in ratingDb.Favorites where item == CurrentFavoritesItem select item).FirstOrDefault();
            if (favoritesItem != null)
            {
                favoritesItem.Semestr = RequestManip.Semestr;
                ratingDb.SubmitChanges();
            }
        }

        internal void SaveFavorites(string name)
        {
            ratingDb.SaveFavorites(RequestManip, name);
        }

        public bool CheckFavorites()
        {
            return ratingDb.CheckFavorites(RequestManip);
        }
    }
}