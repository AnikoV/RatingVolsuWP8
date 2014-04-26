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

        public async Task<ObservableCollection<Facult>> GetFucultList()
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/facult_req.php";
            _data = new Data
            {
                {"get_lists", "0"}
            };
            string content = await SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));

            var facults = new ObservableCollection<Facult>(JsonConvert.DeserializeObject<ObservableCollection<Facult>>(content));
            return facults;

        }

        public async Task<ObservableCollection<Group>> GetGroupList(string FacultId)
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/group_req.php";
            _data = new Data
            {
                {"fak_id", FacultId}
            };
            string content = await SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));

            var groups = new ObservableCollection<Group>(JsonConvert.DeserializeObject<ObservableCollection<Group>>(content));
            return groups;
        }

        public async Task<ObservableCollection<Student>> GetStudentList(string GroupId)
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/sem_req.php";
            _data = new Data
            {
                {"group_id", GroupId}
            };
            string content = await SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));

            var students = new ObservableCollection<Student>(JsonConvert.DeserializeObject<ObservableCollection<Student>>(content));
            return students;
        }

        public async Task<ObservableCollection<Facult>> GetRatingOfGroup(string FacultId, string GroupId, string Semestr)
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/group_rat.php";
            _data = new Data
            {
                {"Fak", FacultId},
                {"Group", GroupId},
                {"Semestr", Semestr}
            };
            string content = await SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));

            var students = new ObservableCollection<Facult>(JsonConvert.DeserializeObject<ObservableCollection<Facult>>(content));
            return students;
        }

        public async Task<StudentRat> GetRatingOfStudent(string FacultId, string GroupId, string Semestr, string Zach)
        {
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/stud_rat.php";
            _data = new Data
            {
                {"Fak", FacultId},
                {"Group", GroupId},
                {"Semestr", Semestr},
                {"Zach", Zach}
            };
            string content = await SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));

            var rating = JsonConvert.DeserializeObject<StudentRat>(content);
            return rating;
        }
    }
}
