using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Newtonsoft.Json;
using RatingVolsuAPI;
using RatingVolsuAPI;

namespace RatingVolsuWP8
{
    public class RatingViewModel : PropertyChangedBase
    {
        private ObservableCollection<Facult> _facultCollection;
        public ObservableCollection<Facult> facultCollection
        {
            get { return _facultCollection; }
            set
            {
                _facultCollection = value;
                RaisePropertyChanged("facultCollection");
            }
        }
        public ObservableCollection<Group> _groupCollection;
        public ObservableCollection<Group> groupCollection
        {
            get { return _groupCollection; }
            set
            {
                _groupCollection = value;
                RaisePropertyChanged("groupCollection");
            }
        }
        public ObservableCollection<Student> _studentCollection;
        public ObservableCollection<Student> studentCollection
        {
            get { return _studentCollection; }
            set
            {
                _studentCollection = value;
                RaisePropertyChanged("studentCollection");
            }
        }
       
        public string FacultId;
        public string GroupId;
        public string StudentId;
        public string Semestr;

        private RatingDatabase rating;
        private RequestManager request;

        public RatingViewModel(string toDoDBConnectionString)
        {
            rating = new RatingDatabase(toDoDBConnectionString);
            LoadCollectionsFromDatabase();
            request = new RequestManager();
        }

        public void SaveChangesRatingtoDb(string favoritesId)
        {
            var currentFavorites = rating.Favorites.FirstOrDefault(x => x.Id == favoritesId);
            if (currentFavorites == null)
            {
                rating.Favorites.InsertOnSubmit(new FavoritesItem()
                {
                    Id = StudentId,
                    Type = RatingType.RatingOfStudent,
                    Student = studentCollection.First(x => x.Id == StudentId)

                });
                
                foreach (var facult in facultCollection)
                {
                    if (rating.Facults.FirstOrDefault(x => x.Id == facult.Id) == null)
                        rating.Facults.InsertOnSubmit(facult);
                }

                foreach (var group in groupCollection)
                {
                    group.FacultId = FacultId;
                    if (rating.Groups.FirstOrDefault(x => x.Id == group.Id) == null)
                    {
                        rating.Groups.InsertOnSubmit(group);
                    }
                        
                }

                foreach (var student in studentCollection)
                {
                    student.GroupId = GroupId;
                    if (rating.Students.FirstOrDefault(x => x.Id == student.Id) == null)
                        rating.Students.InsertOnSubmit(student);
                }
                rating.SubmitChanges();
            } 
        }
        
        public void LoadCollectionsFromDatabase()
        {
            var students = from Student item in rating.Students.Where(x => x.GroupId == GroupId)
                           select item;

            studentCollection = new ObservableCollection<Student>(students);

            var groups = from Group item in rating.Groups
                         select item;
            groupCollection = new ObservableCollection<Group>(groups);

            var facults = from Facult item in rating.Facults
                                select item;
            facultCollection = new ObservableCollection<Facult>(facults);

            

            
        }

        public async Task GetFacults()
        {
           facultCollection = await request.GetFucultList();
        }

        public async Task GetGroups(int SelectedId)
        {
            FacultId = facultCollection[SelectedId].Id;
            groupCollection = await request.GetGroupList(FacultId);
        }

        public async Task GetStudents(int SelectedId)
        {
            studentCollection = await request.GetStudentList(GroupId);
        }

        public int GetSemestrCount()
        { 
            int Period = DateTime.Now.Year - Convert.ToInt32(groupCollection.FirstOrDefault(x => x.Id == GroupId).Year);
            Period *= 2;
            return Period;
        }
    }
}
