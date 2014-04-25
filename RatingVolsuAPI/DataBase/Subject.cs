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

        [Column(IsVersion = true)]
        private Binary _version;

        #region relation rating-subject
        private EntitySet<Rating> _ratings;

        [Association(Storage = "_subj", OtherKey = "SubjectId", ThisKey = "Id")]
        public EntitySet<Rating> Ratings
        {
            get { return this._ratings; }
            set { this._ratings.Assign(value); }
        }

        public Subject()
        {
            _ratings = new EntitySet<Rating>(
                new Action<Rating>(this.attach_subj),
                new Action<Rating>(this.detach_subj)
                );
        }

        private void attach_subj(Rating subj)
        {
            NotifyPropertyChanging("Rating");
            subj.Subject = this;
        }

        private void detach_subj(Rating subj)
        {
            NotifyPropertyChanging("Rating");
            subj.Subject = null;
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
