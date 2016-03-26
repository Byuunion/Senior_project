using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;


namespace MeetMeet_Native_Portable
{
    public class Updater 
    {
        public static async Task<Boolean> UpdateObject(Updatable obj, string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var uri = new Uri(url + obj.GetName());
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var task = client.PutAsync(uri, content);



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
