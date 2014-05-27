using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatingVolsuAPI;
using RatinVolsuAPI;

namespace RatingVolsuWP8
{
    internal class InputDataViewModel : PropertyChangedBase
    {
        //Settings
        public InputDataMode CurrentInputMode;
        public RatingType CurrentRatingType;
        public RequestManipulation RequestManip;
        private readonly RequestManager _requestManager;

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

        public async Task GetStudents(int selectedIndex)
        {
            Students = await _requestManager.GetStudentList(RequestManip.GroupId);
        }
        #endregion

        
    }
}