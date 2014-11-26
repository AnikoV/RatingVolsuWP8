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
        public delegate void Handler(int count);
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

        public async Task GetFacults(Handler handler)
        {
            Facults = await _requestManager.GetFacultList();
            handler(Facults.Count);
        }

        public async Task GetGroups(int selectedIndex, Handler handler)
        {
            RequestManip.FacultId = Facults[selectedIndex].Id;
            Groups = await _requestManager.GetGroupList(RequestManip.FacultId);
            handler(Groups.Count);
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

        public async Task GetStudents(Handler handler)
        {
            Students = await _requestManager.GetStudentList(RequestManip.GroupId);
            handler(Students.Count);
        }
        #endregion

        public async Task<List<string>> GetSemestrList(string groupId, Handler handler)
        {
            var list = await _requestManager.GetSemestrList(groupId);
            handler(list.Count);
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