using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using RatingVolsuAPI;

namespace ForTesting
{
    public class InputDataViewModel : PropertyChangedBase
    {
        private ObservableCollection<Facult> _facultCollection;
        public ObservableCollection<Facult> facultCollection
        {
            get { return _facultCollection; }
            set
            {
                _facultCollection = value;
                RaisePropertyChanged("facultCollection");
            }
        }

        public ObservableCollection<Group> _groupCollection;
        public ObservableCollection<Group> groupCollection
        {
            get { return _groupCollection; }
            set
            {
                _groupCollection = value;
                RaisePropertyChanged("groupCollection");
            }
        }

        public ObservableCollection<Student> _studentCollection;
        public ObservableCollection<Student> studentCollection
        {
            get { return _studentCollection; }
            set
            {
                _studentCollection = value;
                RaisePropertyChanged("studentCollection");
            }
        }

        private List<string> _semestrCollection;
        public List<string> semestrCollection
        {
            get { return _semestrCollection; }
            set
            {
                _semestrCollection = value;
                RaisePropertyChanged("semestrCollection");
            }
        }

        private readonly RequestManager request;
        public RequestManipulation RequestInfo;

        public InputDataViewModel(string toDoDBConnectionString)
        {
            LoadCollectionsFromDatabase();
            request = new RequestManager();
            facultCollection = new ObservableCollection<Facult>();
            groupCollection = new ObservableCollection<Group>();
            studentCollection = new ObservableCollection<Student>();
        }

        public void LoadCollectionsFromDatabase()
        {
            //var students = from Student item in rating.Students.Where(x => x.GroupId == RequestInfo.GroupId)
            //               select item;

            //studentCollection = new ObservableCollection<Student>(students);

            //var groups = from Group item in rating.Groups
            //             select item;
            //groupCollection = new ObservableCollection<Group>(groups);

            //var facults = from Facult item in rating.Facults
            //              select item;
            //facultCollection = new ObservableCollection<Facult>(facults);
        }

        public async Task GetFacults()
        {
            facultCollection = await request.GetFacultList();
        }

        public async Task GetGroups(int selectedId)
        {
            RequestInfo.FacultId = facultCollection[selectedId].Id;
            groupCollection = await request.GetGroupList(RequestInfo.FacultId);
        }

        public async Task GetSemestrList(int selectedIndex)
        {
            RequestInfo.GroupId = groupCollection[selectedIndex].Id;
            semestrCollection = await request.GetSemestrList(RequestInfo.GroupId);
        }

        public async Task GetStudents(int selectedId)
        {
            studentCollection = await request.GetStudentList(RequestInfo.GroupId);
        }
    }
}
