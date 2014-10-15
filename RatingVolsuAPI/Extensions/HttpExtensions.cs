using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
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
                catch (WebException)
                {
                    Debug.WriteLine("exception in GetRequestStreamAsync");
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
                    Debug.WriteLine("exception in GetResponseAsync");
                    HttpWebResponse failedResponse = (HttpWebResponse)webExc.Response;
                    taskComplete.TrySetResult(failedResponse);
                }
            }, request);
            return taskComplete.Task;
        }
        public static async Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request, CancellationToken ct)
        {
             
            using (ct.Register(() =>
            {
                request.Abort();
                Debug.WriteLine("Request was aborted");
            }, useSynchronizationContext: false))
            {
                try
                {
                    var response = await request.GetResponseAsync();
                    ct.ThrowIfCancellationRequested();
                    return (HttpWebResponse)response;
                }
                catch (WebException ex)
                {
                    // WebException is thrown when request.Abort() is called,
                    // but there may be many other reasons,
                    // propagate the WebException to the caller correctly

                    if (ct.IsCancellationRequested)
                    {
                        // the WebException will be available as Exception.InnerException
                        throw new OperationCanceledException(ex.Message, ex, ct);
                    }

                    // cancellation hasn't been requested, rethrow the original WebException
                    throw;
                }
            }
        }
    }
}