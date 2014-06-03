using RatingVolsuAPI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using RatingVolsuAPI;

namespace RatingVolsuAPI.DataBase
{
    [Table]
    public class Semestr : PropertyChangedBase, INotifyPropertyChanging
    {
        private int _id;

        [Column(IsPrimaryKey = true, DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                RaisePropertyChanged("Id");
            }
        }

        private string _number;
        
        [Column]
        public string Number {
            get { return _number; }
            set
            {
                _number = value;
                RaisePropertyChanged("Number");
            }
        }

        private string _groupId;
        [Column]
        public string GroupId {
            get { return _groupId; }
            set
            {
                _groupId = value;
                RaisePropertyChanged("GroupId");
            }
        }

        public Group GroupItem { get; set; }
        

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
