﻿using RatingVolsuAPI.Base;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using RatingVolsuAPI.DataBase;


namespace RatingVolsuAPI
{
    [Table]
    public class Facult : PropertyChangedBase, IRepository, INotifyPropertyChanging
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

        private string _facultName;
        
        [Column]
        public string Name {
            get { return _facultName; }
            set
            {
                _facultName = value;
                RaisePropertyChanged("Name");
            }
        }

        public void Update(IRepository item)
        {
            
        }

        #region relation facult-group

        private EntitySet<Group> _groups;

        [Association(Storage = "_groups", OtherKey = "FacultId", ThisKey = "Id")]
        public EntitySet<Group> Group
        {
            get { return this._groups; }
            set { this._groups.Assign(value); }
        }

        public Facult()
        {
            _groups = new EntitySet<Group>(
                new Action<Group>(this.attach_group),
                new Action<Group>(this.detach_group)
                );
        }

        private void attach_group(Group group)
        {
            NotifyPropertyChanging("Group");
            group.Facult = this;
        }

        private void detach_group(Group group)
        {
            NotifyPropertyChanging("Group");
            group.Facult = null;
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
