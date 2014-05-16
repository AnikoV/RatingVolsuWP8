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
using RatinVolsuAPI;
using WinPhoneExtensions;

using Data = System.Collections.Generic.Dictionary<string, string>;
using FacultCollection = System.Collections.Generic.Dictionary<string, RatingVolsuAPI.Facult>;

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
            rating = new RatingDatabase(Info.DbConnectionString);
        }

        private async Task<string> SendRequest(string DataRequest)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
            HttpExtensions.PostString = DataRequest;
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            
            try
            {
                var webRequest = (HttpWebRequest) await myHttpWebRequest.GetRequestStreamAsync();
                HttpWebResponse response = (HttpWebResponse) await webRequest.GetResponseAsync();
                Debug.WriteLine(myHttpWebRequest.ContentType);
                Stream responseStream = response.GetResponseStream();
                string content;
                using (var reader = new StreamReader(responseStream))
                {
                    content = reader.ReadToEnd();
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
                    MessageBox.Show("RespCallback Exception raised! Message:{0}" + we.Message);
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
        public async Task<ObservableCollection<Facult>> GetFucultList()
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/facult_req.php";
            _data = new Data
            {
                {"get_lists", "0"}
            };
            string content = await SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));
            
            var facults = new ObservableCollection<Facult>(JsonConvert.DeserializeObject<ObservableCollection<Facult>>(content));
            foreach (var facult in facults)
            {
                if (rating.Facults.FirstOrDefault(x => x.Id == facult.Id) == null)
                    rating.Facults.InsertOnSubmit(facult);
            }
            rating.SubmitChanges();
            return facults;

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
            string content = await SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));

            var groups = new ObservableCollection<Group>(JsonConvert.DeserializeObject<ObservableCollection<Group>>(content));
            foreach (var group in groups)
            {
                group.FacultId = FacultId;
                if (rating.Groups.FirstOrDefault(x => x.Id == group.Id) == null)
                {
                    rating.Groups.InsertOnSubmit(group);
                }

            }
            rating.SubmitChanges();
            groups = rating.LoadGroups(FacultId);
            return groups;
        }

        /// <summary>
        /// Выполняет запрос на сервер для получения списка студентов по заданной группе
        /// </summary>
        /// <param name="GroupId">Идентификатор группы</param>
        /// <returns>Коллекция студентов</returns>
        public async Task<ObservableCollection<Student>> GetStudentList(string GroupId)
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/sem_req.php";
            _data = new Data
            {
                {"group_id", GroupId}
            };
            string content = await SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));

            var students = new ObservableCollection<Student>(JsonConvert.DeserializeObject<ObservableCollection<Student>>(content));

            foreach (var student in students)
            {
                student.GroupId = GroupId;
                if (rating.Students.FirstOrDefault(x => x.Id == student.Id) == null)
                    rating.Students.InsertOnSubmit(student);
            }
            rating.SubmitChanges();
            students = rating.LoadStudents(GroupId);
            return students;
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
            string content = await SendRequest(parameters);

            var groupRating = JsonConvert.DeserializeObject<GroupRat>(content);
            return requestInfo.GetRatingFromServer(groupRating);
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
            string content = await SendRequest(parametrs);

            var studentRating = JsonConvert.DeserializeObject<StudentRat>(content);
            return requestInfo.GetRatingFromServer(studentRating);
        }

        public async Task<string> GetRatingCurrentYear()
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/current_year.php";
            string content = await SendRequest("");
            return content;
            
        }        
    }
}
