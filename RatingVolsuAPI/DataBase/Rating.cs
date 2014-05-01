using RatingVolsuAPI.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatingVolsuAPI
{
    [Table]
    public class Rating : PropertyChangedBase, INotifyPropertyChanging
    {
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL IDENTITY", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("Id");
                    _id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        private string _semestr;
       
        [Column]
        public string Semestr
        {
            get { return _semestr; }
            set
            {
                if (_semestr != value)
                {
                    NotifyPropertyChanging("Semestr");
                    _semestr = value;
                    RaisePropertyChanged("Semestr");
                }
            }
        }

        private string _subjectId;

        [Column]
        public string SubjectId
        {
            get { return _subjectId; }
            set
            {
                NotifyPropertyChanging("SubjectId");
                _subjectId = value;
                RaisePropertyChanged("SubjectId");
            }
        }

        private EntityRef<Subject> _subject;

        [Association(Storage = "_subject", ThisKey = "SubjectId", OtherKey = "Id", IsForeignKey = true)]

        public Subject Subject
        {
            get { return _subject.Entity; }
            set
            {
                RaisePropertyChanged("Subject");
                _subject.Entity = value;

                if (value != null)
                {
                    SubjectId = value.Id;
                }

                NotifyPropertyChanging("Subject");
            }
        }

        [Column]
        public string StudentId;

        private EntityRef<Student> _student;

        [Association(Storage = "_student", ThisKey = "StudentId", OtherKey = "Id", IsForeignKey = true)]

        public Student Student
        {
            get { return _student.Entity; }
            set
            {
                RaisePropertyChanged("Student");
                _student.Entity = value;

                if (value != null)
                {
                    StudentId = value.Id;
                }

                NotifyPropertyChanging("Student");
            }
        }

        private string _att1;
        

        [Column]
        public string Att1
        {
            get { return _att1; }
            set
            {
                _att1 = value;
                RaisePropertyChanged("Att1");
            }
        }

        private string _att2;

        [Column]
        public string Att2
        {
            get { return _att2; }
            set
            {
                _att2 = value;
                RaisePropertyChanged("Att2");
            }
        }

        private string _att3;

        [Column]
        public string Att3
        {
            get { return _att3; }
            set
            {
                _att3 = value;
                RaisePropertyChanged("Att3");
            }
        }

        private string _sum;

        [Column]
        public string Sum
        {
            get { return _sum; }
            set
            {
                _sum = value;
                RaisePropertyChanged("Sum");
            }
        }

        private string _exam;

        [Column]
        public string Exam
        {
            get { return _exam; }
            set
            {
                _exam = value;
                RaisePropertyChanged("Exam");
            }
        }

        private string _total;

        [Column]
        public string Total
        {
            get { return _total; }
            set
            {
                _total = value;
                RaisePropertyChanged("Total");
            }
        }

        private string _type;

        [Column]
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged("Type");
            }
        }

        [Column(IsVersion = true)]
        private Binary _version;

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
