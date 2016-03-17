using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace MeetMeet_Native_Portable
{
    public class Poster
    {

        public static async Task<bool> PostObject(Postable obj, string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var uri = new Uri(string.Format(url, obj.getName()));
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var task = client.PostAsync(uri, content);

            

            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                System.Diagnostics.Debug.WriteLine("Response received");
                System.Diagnostics.Debug.WriteLine(task.Status);

            }
            else {
                System.Diagnostics.Debug.WriteLine("Response timeout");
            }


            if (task.IsCompleted)
            {
                System.Diagnostics.Debug.WriteLine("Successful");
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Not Successful");
                return false;
            }
        }


    }


}
