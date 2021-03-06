﻿using RatingVolsuAPI;
using RatingVolsuAPI.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;


namespace RatingVolsuAPI
{
    [Table]
    public class FavoritesItem : PropertyChangedBase, IRepository, INotifyPropertyChanging
    {
        private string _id;

        [Column(IsPrimaryKey = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged("Id");
            }
        }

        private string _semestr;
        [Column]
        public string Semestr
        {
            get { return _semestr; }
            set
            {
                NotifyPropertyChanging("Semestr");
                _semestr = value;
                RaisePropertyChanged("Semestr");
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
        public void Update(IRepository item)
        {

        }

        #region relation student-FavoriteItem

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

        #endregion

        #region relation group-FavoriteItem

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

        public string _name;
        [Column]
        public string Name
        {
            get { return _name; }
            set
            {
                NotifyPropertyChanging("Name");
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string SubName
        {
            get
            {
                try
                {
                    if (Type == RatingType.RatingOfGroup)
                        return Group.Name + " Семестр " + Semestr;
                    return Student.Group.Name + " Семестр " + Semestr + " " + Student.Number;
                }
                catch (Exception)
                {
                    return "";
                }
                
            }
            private set { }
        }

        public RequestManipulation GetRequest()
        {
            if (Type == RatingType.RatingOfStudent)
                return new RequestByStudent()
                {
                    FacultId = Student.Group.FacultId,
                    GroupId = Student.GroupId,
                    Semestr = Semestr,
                    StudentId = Student.Id
                };
            return new RequestByGroup()
            {
                FacultId = Group.FacultId,
                GroupId = GroupId,
                Semestr = Semestr
            };
        }
    }
}
