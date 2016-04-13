using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace MeetMeet_Native_Portable
{
    public class Poster
    {

        public static async Task<Boolean> PostObject(Object obj, string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var uri = new Uri(url);
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var task = client.PostAsync(uri, content);
            
			System.Diagnostics.Debug.WriteLine("Trying to post object: " + json + " to: " + uri);

            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                string response = await task.Result.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("Post response received. String is: " + response);
                return task.Result.IsSuccessStatusCode && !response.Contains("\"success\":false");
            }
            else {
                System.Diagnostics.Debug.WriteLine("Response timeout");
                return false;
            }
        }

    }


}
