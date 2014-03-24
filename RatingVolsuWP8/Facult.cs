using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RatingVolsuWP8
{
    public class SavedRequest
    {
        public ObservableCollection<Facult> FacultCollection;
        public ObservableCollection<Group> GroupCollection;
        public ObservableCollection<Student> StudentCollection;
        public StudentRat RatingStudent;
        public string FacultId;
        public string GroupId;
        public string StudentId;
        public string Semestr;

        public void SetValues(RatingViewModel v)
        {
            FacultCollection = new ObservableCollection<Facult>(v.facults);
            GroupCollection = new ObservableCollection<Group>(v.groups);
            StudentCollection = new ObservableCollection<Student>(v.students);
            RatingStudent = v.RatingStudent;
            FacultId = v.FacultId;
            GroupId = v.GroupId;
            StudentId = v.StudentId;
            Semestr = v.Semestr;
        }
    }

    public class Facult : INotifyPropertyChanged
    {
        public string Name;
        public string Id;
        public ObservableCollection<Group> Groups;

        public string FacultName
        {
            get { return Name; }
            set
            {
                Name = value;
                NotifyPropertyChanged("FacultName");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Group : INotifyPropertyChanged
    {
        public string Name;

        public string Year;

        public string Id;

        public ObservableCollection<Student> Students;

        public string GroupName
        {
            get { return Name; }
            set
            {
                Name = value;
                NotifyPropertyChanged("GroupName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public class Student : INotifyPropertyChanged
    {
        public string Id;
        public string Number;

        public string StudentNumber
        {
            get { return Number; }
            set
            {
                Number = value;
                NotifyPropertyChanged("StudentNumber");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class RatingOfStudent
    {
        public string SubjectId;
        public int Att1;
        public int Att2;
        public int Att3;
        public int Sum;
        public int Exam;


    }

    public class RatingOfGroup
    {
        public string studentId;
        public string numberStudent;
        public Dictionary<string, string> subjects;
    }

    public class Subject
    {
        public string Id;
        public string Name;
    }

    public class StudentRat
    {
        public Dictionary<string, string> Predmet;
        public Dictionary<string, List<string>> Table;
    }
}
