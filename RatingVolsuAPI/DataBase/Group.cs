using System.Linq;
using RatingVolsuAPI.Base;
using RatingVolsuAPI.DataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;


namespace RatingVolsuAPI
{
    [Table]
    public class Group : PropertyChangedBase, IRepository, INotifyPropertyChanging
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

        private string _type;

        [Column]
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value.Contains("спец") ? "специалитет" : value;
                RaisePropertyChanged("Type");
            }
        }

        public List<string> SemList
        {
            get { return Semestr.ToList().Select(x => x.Number).ToList(); }
        }

        public void Update(IRepository item)
        {

        }

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

        #region relation group-semestr
        private EntitySet<Semestr> _semestr;

        [Association(Storage = "_semestr", OtherKey = "GroupId", ThisKey = "Id")]

        internal EntitySet<Semestr> Semestr
        {
            get { return this._semestr; }
            set { this._semestr.Assign(value); }
        }

        public Group()
        {
            _semestr = new EntitySet<Semestr>(
                new Action<Semestr>(this.attach_semestr),
                new Action<Semestr>(this.detach_semestr)
                );
        }

        private void attach_semestr(Semestr sem)
        {
            NotifyPropertyChanging("Semestr");
            sem.GroupItem = this;
        }

        private void detach_semestr(Semestr sem)
        {
            NotifyPropertyChanging("Semestr");
            sem.GroupItem = null;
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
