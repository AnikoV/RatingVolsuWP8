using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RatingVolsuAPI;
using WinPhoneExtensions;

using Data = System.Collections.Generic.Dictionary<string, string>;
using FacultCollection = System.Collections.Generic.Dictionary<string, RatingVolsuAPI.Facult>;
using RatingVolsuAPI.DataBase;

namespace RatingVolsuAPI
{
    public delegate void DataLoaded(string content);

    public class RequestManager
    {
        private string _url;
        private Data _data;
        private RatingDatabase rating;

        public RequestManager()
        {
            rating = new RatingDatabase();
        }

        private async Task<string> SendRequest(string DataRequest)
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
            HttpExtensions.PostString = DataRequest;
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            
            try
            {
                var webRequest = (HttpWebRequest) await myHttpWebRequest.GetRequestStreamAsync();
                var response = (HttpWebResponse) await webRequest.GetResponseAsync();
                Debug.WriteLine(myHttpWebRequest.ContentType);
                Stream responseStream = response.GetResponseStream();
                string content;
                using (var reader = new StreamReader(responseStream))
                {
                    content = reader.ReadToEnd();
                    Debug.WriteLine("response:\n {0}" + content);
                }
                responseStream.Close();
                return content;
            }
            catch (Exception ex)
            {
                var we = ex.InnerException as WebException;
                if (we != null)
                {
                    var resp = we.Response as HttpWebResponse;
                    var code = resp.StatusCode;
                    Debug.WriteLine("RespCallback Exception raised! Message:{0}" + we.Message);
                    Debug.WriteLine("Status:{0}", we.Status);
                }
                else
                    throw;
                return null;
            }
        }
        /// <summary>
        /// Выполняет запрос на сервер для получения списка факультетов 
        /// </summary>
        /// <returns>Коллекция факультетов</returns>
        public async Task<ObservableCollection<Facult>> GetFacultList()
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/facult_req.php";
            _data = new Data
            {
                {"get_lists", "0"}
            };
            var parameters = string.Join("&", _data.Select(v => v.Key + "=" + v.Value));
            Debug.WriteLine("Sending request - get facults\n With params " + parameters);
            string content = await SendRequest(parameters);
            try
            {
                if (!String.IsNullOrEmpty(content))
                {
                    var facults = new ObservableCollection<Facult>(JsonConvert.DeserializeObject<ObservableCollection<Facult>>(content));
                    Debug.WriteLine("Parsed object:\n" + facults);
                    Debug.WriteLine("Saving in database..\n");
                    foreach (var facult in facults)
                    {
                        if (rating.Facults.FirstOrDefault(x => x.Id == facult.Id) == null)
                            rating.Facults.InsertOnSubmit(facult);
                    }
                    rating.SubmitChanges();
                    Debug.WriteLine("Done\n");
                    return facults;
                }
            }
            catch (Exception)
            {
                new ObservableCollection<Facult>();
            }
            return null;
        }

        /// <summary>
        ///  Выполняет запрос на сервер для получения списка групп по заданному факультету
        /// </summary>
        /// <param name="FacultId">Идентификатор факультета</param>
        /// <returns>Коллекция групп</returns>
        public async Task<ObservableCollection<Group>> GetGroupList(string FacultId)
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/group_req.php";
            _data = new Data
            {
                {"fak_id", FacultId}
            };

            string parameters = string.Join("&", _data.Select(v => v.Key + "=" + v.Value));
            Debug.WriteLine("Sending request - get groups\n With params " + parameters);
            string content = await SendRequest(parameters);
            try
            {
                var groups =
                    new ObservableCollection<Group>(JsonConvert.DeserializeObject<ObservableCollection<Group>>(content));
                Debug.WriteLine("Parsed object:\n" + groups);
                Debug.WriteLine("Saving in database..\n");
                foreach (var group in groups)
                {
                    group.FacultId = FacultId;
                    if (rating.Groups.FirstOrDefault(x => x.Id == group.Id) == null)
                    {
                        rating.Groups.InsertOnSubmit(group);
                    }

                }
                rating.SubmitChanges();
                Debug.WriteLine("Done\n");
                groups = rating.LoadGroups(FacultId);
                return groups;
            }
            catch (Exception)
            {
                return new ObservableCollection<Group>();
            }
        }

        /// <summary>
        ///  Выполняет запрос на сервер для получения списка семестров по заданной группе
        /// </summary>
        /// <param name="groupId"> Идентификатор группы</param>
        /// <returns>Коллекция групп</returns>
        public async Task<List<string>> GetSemestrList(string groupId)
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/sem_req.php";
            _data = new Data
            {
                {"gr_id", groupId}
            };
            string parameters = string.Join("&", _data.Select(v => v.Key + "=" + v.Value));
            Debug.WriteLine("Sending request - get semestrs\n With params " + parameters);
            string content = await SendRequest(parameters);
            try
            {
                var semNumbers =
                    new List<string>(JsonConvert.DeserializeObject<List<string>>(content));
                Debug.WriteLine("Parsed object:\n" + semNumbers);
                Debug.WriteLine("Saving in database..\n");
                foreach (var item in semNumbers)
                {
                    var sem = new Semestr()
                    {
                        Id = groupId + item,
                        Number = item,
                        GroupId = groupId
                    };
                    if (rating.SemestrItems.FirstOrDefault(x => x.Number == item && x.GroupId == groupId ) == null)
                    {
                        rating.SemestrItems.InsertOnSubmit(sem);
                    }

                }
                rating.SubmitChanges();
                Debug.WriteLine("Done\n");
                var group = rating.Groups.FirstOrDefault(x => x.Id == groupId);
                
                return group.SemList;
            }
            catch (Exception)
            {
                return new List<string>();
            }

        }

        /// <summary>
        /// Выполняет запрос на сервер для получения списка студентов по заданной группе
        /// </summary>
        /// <param name="GroupId">Идентификатор группы</param>
        /// <returns>Коллекция студентов</returns>
        public async Task<ObservableCollection<Student>> GetStudentList(string GroupId)
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/stud_req.php";
            _data = new Data
            {
                {"group_id", GroupId}
            };
            string parameters = string.Join("&", _data.Select(v => v.Key + "=" + v.Value));
            Debug.WriteLine("Sending request - get students\n With params " + parameters);
            string content = await SendRequest(parameters);
            Debug.WriteLine(content);
            try
            {
                var students = new ObservableCollection<Student>(JsonConvert.DeserializeObject<ObservableCollection<Student>>(content));
                Debug.WriteLine("Parsed object: " + students);
                Debug.WriteLine("Saving in database..\n");
                foreach (var student in students)
                {
                    student.GroupId = GroupId;
                    if (rating.Students.FirstOrDefault(x => x.Id == student.Id) == null)
                        rating.Students.InsertOnSubmit(student);
                }
                rating.SubmitChanges();
                Debug.WriteLine("Done\n");
                students = rating.LoadStudents(GroupId);
                return students;
            }
            catch (Exception)
            {
                return new ObservableCollection<Student>(); ;
            }
            
        }

        /// <summary>
        /// Выполняет запрос на рейтинг по группе
        /// </summary>
        /// <param name="requestInfo">Объект, содержащий параметры запроса</param>
        /// <returns>Коллекция рейтинга по группе</returns>
        public async Task<ObservableCollection<Rating>> GetRatingOfGroup(RequestManipulation requestInfo)
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/group_rat.php";
            var parameters = requestInfo.GetParams();
            Debug.WriteLine("Sending request - rating of group\n With params " + parameters);
            string content = await SendRequest(parameters);
            try
            {
                var groupRating = JsonConvert.DeserializeObject<GroupRat>(content);
                Debug.WriteLine("{0}", groupRating);
                Debug.WriteLine("Begin database saving..\n");
                var result = requestInfo.GetRatingFromServer(groupRating);
                Debug.WriteLine("Done\n");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        /// <summary>
        /// Выполняет запрос на рейтинг по студенту
        /// </summary>
        /// <param name="requestInfo">Объект, содержащий параметры запроса</param>
        /// <returns></returns>
        public async Task<ObservableCollection<Rating>> GetRatingOfStudent(RequestManipulation requestInfo)
        {
            var req = (RequestByStudent) requestInfo;
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/stud_rat.php";

            var parametrs = req.GetParams();
            Debug.WriteLine("Sending request - rating of student\n With params " + parametrs);
            string content = await SendRequest(parametrs);
            try
            {
                var studentRating = JsonConvert.DeserializeObject<StudentRat>(content);
                Debug.WriteLine("{0}", JsonConvert.SerializeObject(studentRating));
                Debug.WriteLine("Begin database saving..\n");
                var result = requestInfo.GetRatingFromServer(studentRating);
                Debug.WriteLine("Done\n");
                return result;

            }
            catch (Exception)
            {
                return null; 
            }
        }

        public async Task<string> GetRatingCurrentYear()
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/current_year.php";
            string content = await SendRequest("");
            return content;
        }
    }
}
