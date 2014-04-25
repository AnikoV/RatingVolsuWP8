using RatingVolsuAPI;
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
    public class FavoritesItem : PropertyChangedBase, INotifyPropertyChanging
    {
        private string _id;

        [Column(IsPrimaryKey = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string Id
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                RaisePropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Name");
                _id = value;
                RaisePropertyChanged("Name");
            }
        }

        private RatingType _type;

        [Column]
        public RatingType Type
        {
            get { return _type; }
            set
            {
                NotifyPropertyChanging("Type");
                _type = value;
                RaisePropertyChanged("Type");
            }
        }

        #region relation student-FavoriteItem

        [Column]
        internal string _studentId;

        private EntityRef<Student> _student;

        [Association(Storage = "_student", ThisKey = "_studentId", OtherKey = "Id", IsForeignKey = true)]

        public Student Student
        {
            get { return _student.Entity; }
            set
            {
                RaisePropertyChanged("Student");
                _student.Entity = value;

                if (value != null)
                {
                    _studentId = value.Id;
                }

                NotifyPropertyChanging("Student");
            }
        }

        #endregion

        #region relation group-FavoriteItem

        [Column]
        internal string _groupId;

        private EntityRef<Group> _group;

        [Association(Storage = "_group", ThisKey = "_groupId", OtherKey = "Id", IsForeignKey = true)]

        public Group Group
        {
            get { return _group.Entity; }
            set
            {
                RaisePropertyChanged("Group");
                _group.Entity = value;

                if (value != null)
                {
                    _groupId = value.Id;
                }

                NotifyPropertyChanging("Group");
            }
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
