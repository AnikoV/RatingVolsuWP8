using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using RatingVolsuAPI;

namespace RatingVolsuWP8
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
       
        public string FacultId;
        public string GroupId;
        public string StudentId;
        public string Semestr;

        private RatingDatabase rating;
        private RequestManager request;

        public InputDataViewModel(string toDoDBConnectionString)
        {
            rating = new RatingDatabase(toDoDBConnectionString);
            LoadCollectionsFromDatabase();
            request = new RequestManager();
        }
        
        public void LoadCollectionsFromDatabase()
        {
            var students = from Student item in rating.Students.Where(x => x.GroupId == GroupId)
                           select item;

            studentCollection = new ObservableCollection<Student>(students);

            var groups = from Group item in rating.Groups
                         select item;
            groupCollection = new ObservableCollection<Group>(groups);

            var facults = from Facult item in rating.Facults
                                select item;
            facultCollection = new ObservableCollection<Facult>(facults);

            

            
        }

        public async Task GetFacults()
        {
            facultCollection = await request.GetFucultList();
            App.CacheManager.facultCollection = new ObservableCollection<Facult>(facultCollection);
        }

        public async Task GetGroups(int SelectedId)
        {
            FacultId = facultCollection[SelectedId].Id;
            groupCollection = await request.GetGroupList(FacultId);
            App.CacheManager.groupCollection = new ObservableCollection<Group>(groupCollection);
        }

        public async Task GetStudents(int SelectedId)
        {
            studentCollection = await request.GetStudentList(GroupId);
            App.CacheManager.studentCollection = new ObservableCollection<Student>(studentCollection);
        }

        public int GetSemestrCount()
        { 
            int Period = DateTime.Now.Year - Convert.ToInt32(groupCollection.FirstOrDefault(x => x.Id == GroupId).Year);
            Period *= 2;
            return Period;
        }
    }
}
