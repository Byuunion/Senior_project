using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace MeetMeet_Native_Portable
{
    public class Poster
    {

        public static async Task<Boolean> PostObject(Postable obj, string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var uri = new Uri(url);
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var task = client.PostAsync(uri, content);
            
            

            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                System.Diagnostics.Debug.WriteLine("Response received");
                System.Diagnostics.Debug.WriteLine(task.Status);
                return true;

            }
            else {
                System.Diagnostics.Debug.WriteLine("Response timeout");
                return false;
            }
        }

    }


}
