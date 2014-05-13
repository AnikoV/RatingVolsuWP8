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
    public class Subject : PropertyChangedBase, INotifyPropertyChanging
    {

        private string _subjectId;

        [Column(IsPrimaryKey = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string Id
        {
            get { return _subjectId; }
            set
            {
                NotifyPropertyChanging("Id");
                _subjectId = value;
                RaisePropertyChanged("Id");
            }
        }

        private string _subjectName;

        [Column]
        public string Name
        {
            get { return _subjectName; }
            set
            {
                _subjectName = value;
                RaisePropertyChanged("Name");
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
