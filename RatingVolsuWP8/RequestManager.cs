using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Data = System.Collections.Generic.Dictionary<string, string>;
using FacultCollection = System.Collections.Generic.Dictionary<string, RatingVolsuWP8.Facult>;

namespace RatingVolsuWP8
{
    public delegate void DataLoaded(string content);

    public enum RequestType
    {
        Facult,
        Group,
        Student,
        RatingGroup,
        RatingStudent
    }

    public class RequestManager
    {
        private string _url;
        private Data _data;
        private DataLoaded _onDataLoaded;
        public string responseString;
        private string _postString;

        void SendRequest(string DataRequest)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
            _postString = DataRequest;
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            // myHttpWebRequest.AllowAutoRedirect = false;

            myHttpWebRequest.BeginGetRequestStream(RespCallback, myHttpWebRequest);


        }

        private void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
            var webRequest = (HttpWebRequest)asynchronousResult.AsyncState;

            using (var postStream = webRequest.EndGetRequestStream(asynchronousResult))
            {
                var byteArray = new byte[_postString.Length];
                new UTF8Encoding().GetBytes(_postString.ToCharArray(), 0, _postString.Length, byteArray, 0);

                postStream.Write(byteArray, 0, byteArray.Length);
                postStream.Close();

                webRequest.BeginGetResponse(GetResponseCallback, webRequest);
            }

            }
            catch (Exception ex)
            {
                return;
            }

        }

        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
            var webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            using (var response = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult))
            {
                var streamResponse = response.GetResponseStream();
                if (streamResponse != null)
                {
                    var streamReader = new StreamReader(streamResponse);
                    var content = streamReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(content))
                    {
                        _onDataLoaded(content);
                    }
                    streamResponse.Close();
                    streamReader.Close();
                }
            }
            }
            catch(Exception ex)
            {
                return;
            }
        }

        public void GetFacultList(DataLoaded func)
        {
            _onDataLoaded = func;
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/facult_req.php";
            _data = new Data
            {
                {"get_lists", "0"}
            };
            SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));

        }

        public void GetGroupList(DataLoaded func, string FacultId)
        {
            _onDataLoaded = func;
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/group_req.php";
            _data = new Data
            {
                {"fak_id", FacultId}
            };
            SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));
        }

        public void GetStudentList(DataLoaded func, string GroupId)
        {
            _onDataLoaded = func;
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/sem_req.php";
            _data = new Data
            {
                {"group_id", GroupId}
            };
            SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));
        }

        public void GetRatingOfGroup(DataLoaded func, string FacultId, string GroupId, string Semestr)
        {
            _onDataLoaded = func;
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/group_rat.php";
            _data = new Data
            {
                {"Fak", FacultId},
                {"Group", GroupId},
                {"Semestr", Semestr}
            };
            SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));
        }

        public void GetRatingOfStudent(DataLoaded func, string FacultId, string GroupId, string Semestr, string Zach)
        {
            _onDataLoaded = func;
            _url = "http://umka.volsu.ru/newumka3/viewdoc/service_selector/stud_rat.php";
            _data = new Data
            {
                {"Fak", FacultId},
                {"Group", GroupId},
                {"Semestr", Semestr},
                {"Zach", Zach}
            };
            SendRequest(string.Join("&", _data.Select(v => v.Key + "=" + v.Value)));
        }

    }
}
