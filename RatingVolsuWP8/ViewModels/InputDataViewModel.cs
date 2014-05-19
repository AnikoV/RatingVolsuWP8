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
    class InputDataViewModel : PropertyChangedBase
    {
        //Settings
        public InputDataMode CurrentInputMode;
        public RatingType CurrentRatingType;
        public RequestManipulation RequestManip;
        private RequestManager _requestManager;

        //Collections
        public ObservableCollection<Facult> Facults { get; set; }
        public ObservableCollection<Group> Groups { get; set; }
        public ObservableCollection<Student> Students { get; set; }

        public InputDataViewModel()
        {
            Facults = new ObservableCollection<Facult>();
            Groups = new ObservableCollection<Group>();
            Students = new ObservableCollection<Student>();
            _requestManager = new RequestManager();
        }

        #region Requests

        public async Task GetFacults()
        { 
            Facults = await _requestManager.GetFacultList();
        }

        #endregion
    }
}
