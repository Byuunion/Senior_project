using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;


namespace MeetMeet_Native_Portable
{
    /// <summary>
    /// Updater for the user login, because we have to be able to get a token back
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class LoginUpdater<T>
    {

        public static async Task<T> LoginUpdate(Object obj, string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var uri = new Uri(url);

            var json = JsonConvert.SerializeObject(obj);
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpRequestMessage m = new HttpRequestMessage();
            m.Content = content;
            m.Method = HttpMethod.Put;
            m.RequestUri = uri;
            
            var task = client.SendAsync(m);
            
            System.Diagnostics.Debug.WriteLine("Sending login request to: " + uri + " data: " + json);

            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                System.Diagnostics.Debug.WriteLine("Response received");
                return JsonConvert.DeserializeObject<T>(await task.Result.Content.ReadAsStringAsync());
            }
            else {
                System.Diagnostics.Debug.WriteLine("Response timeout");
                return default(T);
            }
        }
    }

}
