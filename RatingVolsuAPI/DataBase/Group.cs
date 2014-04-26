using RatingVolsuAPI.Base;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace RatingVolsuAPI
{
    [Table]
    public class Group : PropertyChangedBase, INotifyPropertyChanging
    {

        private string _groupId;

        [Column(IsPrimaryKey = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string Id
        {
            get { return _groupId; }
            set
            {
                _groupId = value;
                RaisePropertyChanged("Id");
            }
        }

        private string _groupName;

        [Column]
        public string Name
        {
            get { return _groupName; }
            set
            {
                _groupName = value;
                RaisePropertyChanged("Name");
            }
        }

        private string _year;

        [Column]
        public string Year
        {
            get { return _year; }
            set
            {
                _year = value;
                RaisePropertyChanged("Year");
            }
        }

        [Column(IsVersion = true)]
        private Binary _version;

        #region relation facult-group
        [Column]
        public string FacultId;

        private EntityRef<Facult> _facult;

        [Association(Storage = "_facult", ThisKey = "FacultId", OtherKey = "Id", IsForeignKey = true)]

        public Facult Facult
        {
            get { return _facult.Entity; }
            set
            {
                RaisePropertyChanged("Facult");
                _facult.Entity = value;

                if (value != null)
                {
                    FacultId = value.Id;
                }

                NotifyPropertyChanging("Facult");
            }
        }

        #endregion

        #region relation group-student

        private EntitySet<Student> _students;

        [Association(Storage = "_students", OtherKey = "GroupId", ThisKey = "Id")]
        public EntitySet<Student> Student
        {
            get { return this._students; }
            set { this._students.Assign(value); }
        }

        public Group()
        {
            _students = new EntitySet<Student>(
                new Action<Student>(this.attach_stud),
                new Action<Student>(this.detach_stud)
                );
        }

        private void attach_stud(Student student)
        {
            NotifyPropertyChanging("Student");
            student.Group = this;
        }

        private void detach_stud(Student student)
        {
            NotifyPropertyChanging("Student");
            student.Group = null;
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
