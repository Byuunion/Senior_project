using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace MeetMeet_Native_Portable
{
    public class Deleter
    {

        public static async Task DeleteProfile(String username, string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var uri = new Uri(string.Format(url, username));
            var task = client.DeleteAsync(uri);



            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                System.Diagnostics.Debug.WriteLine("Response received");
                System.Diagnostics.Debug.WriteLine(task.Status);

            }
            else {
                System.Diagnostics.Debug.WriteLine("Response timeout");
            }


            if (!task.IsFaulted)
            {
                System.Diagnostics.Debug.WriteLine("Successful");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Not Successful");
            }
        }

    }


}
