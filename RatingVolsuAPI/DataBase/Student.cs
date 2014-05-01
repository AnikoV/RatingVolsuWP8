using RatingVolsuAPI.Base;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace RatingVolsuAPI
{
    [Table]
    public class Student : PropertyChangedBase, INotifyPropertyChanging
    {
        private string _studentId;

        [Column(IsPrimaryKey = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string Id
        {
            get { return _studentId; }
            set
            {
                _studentId = value;
                RaisePropertyChanged("Id");
            }
        }

        private string _number;

        [Column]
        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                RaisePropertyChanged("Number");
            }
        }

        [Column(IsVersion = true)]
        private Binary _version;

        #region relation group-student

        [Column]
        public string GroupId;

        private EntityRef<Group> _group;

        [Association(Storage = "_group", ThisKey = "GroupId", OtherKey = "Id", IsForeignKey = true)]

        public Group Group
        {
            get { return _group.Entity; }
            set
            {
                RaisePropertyChanged("Group");
                _group.Entity = value;

                if (value != null)
                {
                    GroupId = value.Id;
                }

                NotifyPropertyChanging("Group");
            }
        }

        #endregion

        //#region relation rating-students
        //private EntitySet<Rating> _stud;

        //[Association(Storage = "_stud", OtherKey = "StudentId", ThisKey = "Id")]

        //public EntitySet<Rating> Stud
        //{
        //    get { return this._stud; }
        //    set { this._stud.Assign(value); }
        //}

        //public Student()
        //{
        //    _stud = new EntitySet<Rating>(
        //        new Action<Rating>(this.attach_stud),
        //        new Action<Rating>(this.detach_stud)
        //        );
        //}

        //private void attach_stud(Rating stud)
        //{
        //    NotifyPropertyChanging("Rating");
        //    stud.Student = this;
        //}

        //private void detach_stud(Rating stud)
        //{
        //    NotifyPropertyChanging("Rating");
        //    stud.Student = null;
        //}
        //#endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
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
