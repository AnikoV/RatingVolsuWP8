using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinPhoneExtensions
{
    public static class HttpExtensions
    {
        public static string PostString;
        public static Task<HttpWebRequest> GetRequestStreamAsync(this HttpWebRequest request)
        {
            var taskComplete = new TaskCompletionSource<HttpWebRequest>();
            request.BeginGetRequestStream(asyncRequest =>
            {
                try
                {
                    var webRequest = (HttpWebRequest)asyncRequest.AsyncState;

                    using (var postStream = webRequest.EndGetRequestStream(asyncRequest))
                    {
                        var byteArray = new byte[PostString.Length];
                        new UTF8Encoding().GetBytes(PostString.ToCharArray(), 0, PostString.Length, byteArray, 0);
                        postStream.Write(byteArray, 0, byteArray.Length);
                        postStream.Close();
                        taskComplete.TrySetResult(webRequest);
                    }

                }
                catch (WebException webExc)
                {
                    taskComplete.TrySetResult(null);
                }
            }, request);

            return taskComplete.Task;
        } 
        public static Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request)
        {
            var taskComplete = new TaskCompletionSource<HttpWebResponse>();

            request.BeginGetResponse(asyncResponse =>
            {
                try
                {
                    HttpWebRequest responseRequest = (HttpWebRequest)asyncResponse.AsyncState;
                    HttpWebResponse someResponse = (HttpWebResponse)responseRequest.EndGetResponse(asyncResponse);

                    taskComplete.TrySetResult(someResponse);
                }
                catch (WebException webExc)
                {
                    HttpWebResponse failedResponse = (HttpWebResponse)webExc.Response;
                    taskComplete.TrySetResult(failedResponse);
                }
            }, request);
            return taskComplete.Task;
        }
    }
}