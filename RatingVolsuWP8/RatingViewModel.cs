using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Newtonsoft.Json;
using FacultCollection = System.Collections.Generic.Dictionary<string, RatingVolsuWP8.Facult>;
using GroupCollection = System.Collections.Generic.Dictionary<string, RatingVolsuWP8.Group>;
using StudentCollection = System.Collections.Generic.Dictionary<string, RatingVolsuWP8.Student>;

namespace RatingVolsuWP8
{
    public class RatingViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Facult> _facultCollection;
        public ObservableCollection<Facult> facultCollection
        {
            get { return _facultCollection; }
            set
            {
                _facultCollection = value;
                NotifyPropertyChanged("facultCollection");
            }
        }
        public ObservableCollection<Group> _groupCollection;
        public ObservableCollection<Group> groupCollection
        {
            get { return _groupCollection; }
            set
            {
                _groupCollection = value;
                NotifyPropertyChanged("groupCollection");
            }
        }
        public ObservableCollection<Student> _studentCollection;
        public ObservableCollection<Student> studentCollection
        {
            get { return _studentCollection; }
            set
            {
                _studentCollection = value;
                NotifyPropertyChanged("studentCollection");
            }
        }
        public StudentRat RatingStudent;
        public ObservableCollection<Facult> facults;
        public ObservableCollection<Group> groups;
        public ObservableCollection<Student> students;
        public bool IsHiddenFacultList;
        public bool IsHiddenGroupList;
        public bool IsHiddenStudentList;
        public string FacultId;
        public string GroupId;
        public string StudentId;
        public string Semestr;

        public RatingViewModel()
        {
            // facultCollection = new ObservableCollection<Facult>();
        }

        public void GetFacults()
        {
            //   facultCollection.Add(new Facult() { Name = "end" });
            RequestManager request = new RequestManager();
            request.GetFacultList(GetFacultListCallback);
        }

        public void GetGroups(int SelectedId)
        {
            FacultId = facults[SelectedId].Id;
            RequestManager request = new RequestManager();
            request.GetGroupList(GetGroupListCallback, FacultId);
        }

        public void GetStudents(int SelectedId)
        {
            GroupId = groups[SelectedId].Id;
            RequestManager request = new RequestManager();
            request.GetStudentList(GetStudentListCallback, GroupId);
        }

        public void GetRatingOfStudent()
        {
            RequestManager request = new RequestManager();
            request.GetRatingOfStudent(GetRatingOfStudentCallback, FacultId, GroupId, Semestr, StudentId);
        }

        private void GetFacultListCallback(string content)
        {
            facults =
                    new ObservableCollection<Facult>(JsonConvert.DeserializeObject<ObservableCollection<Facult>>(content));
            App.RootFrame.Dispatcher.BeginInvoke(new Action<ObservableCollection<Facult>>(c =>
            {
                facultCollection = new ObservableCollection<Facult>(c);
            }), facults);



        }

        private void GetGroupListCallback(string content)
        {
            groups = new ObservableCollection<Group>(JsonConvert.DeserializeObject<ObservableCollection<Group>>(content));
            App.RootFrame.Dispatcher.BeginInvoke(new Action<ObservableCollection<Group>>(c =>
            {
                groupCollection = new ObservableCollection<Group>(c);
            }), groups);
        }

        private void GetStudentListCallback(string content)
        {
            students = new ObservableCollection<Student>(JsonConvert.DeserializeObject<ObservableCollection<Student>>(content));
            App.RootFrame.Dispatcher.BeginInvoke(new Action<ObservableCollection<Student>>(c =>
            {
                studentCollection = new ObservableCollection<Student>(c);
            }), students);
        }

        void GetRatingOfStudentCallback(string content)
        {
            RatingStudent = JsonConvert.DeserializeObject<StudentRat>(content);
        }

        public void UpdateFacultList(int index)
        {
            facultCollection.Clear();
            if (IsHiddenFacultList)
            {
                facultCollection = new ObservableCollection<Facult>(facults);
                IsHiddenFacultList = false;
            }
            else
            {
                facultCollection.Add(facults[index]);
                IsHiddenFacultList = true;
            }
        }

        public void UpdateGroupList(int index)
        {
            if (groupCollection != null)
            {
                groupCollection.Clear();
                if (IsHiddenGroupList)
                {
                    groupCollection = new ObservableCollection<Group>(groups);
                    IsHiddenGroupList = false;
                }
                else
                {
                    groupCollection.Add(groups[index]);
                    IsHiddenGroupList = true;
                }
            }
        }

        public void UpdateStudentList(int index)
        {
            if (studentCollection != null)
            {
                studentCollection.Clear();
                if (IsHiddenStudentList)
                {
                    studentCollection = new ObservableCollection<Student>(students);
                    IsHiddenFacultList = false;
                }
                else
                {
                    studentCollection.Add(students[index]);
                    IsHiddenFacultList = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        public int GetSemestrCount()
        {
            int Period = DateTime.Now.Year - Convert.ToInt32(groupCollection.FirstOrDefault(x => x.Id == GroupId).Year);
            Period *= 2;
            return Period;
        }

        public void GetValues(SavedRequest sv)
        {
            facultCollection = new ObservableCollection<Facult>(sv.FacultCollection);
            groupCollection = new ObservableCollection<Group>(sv.GroupCollection);
            studentCollection = new ObservableCollection<Student>(sv.StudentCollection);
            facults = new ObservableCollection<Facult>(facultCollection);
            groups = new ObservableCollection<Group>(groupCollection);
            students = new ObservableCollection<Student>(studentCollection);
            RatingStudent = sv.RatingStudent;
            FacultId = sv.FacultId;
            GroupId = sv.GroupId;
            StudentId = sv.StudentId;
            Semestr = sv.Semestr;
        }
    }
}
