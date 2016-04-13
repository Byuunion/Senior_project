using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace MeetMeet_Native_Portable
{
    public class Deleter
    {

		public static async Task<Boolean> DeleteObject(String resource, string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

			var uri = new Uri(url + resource);
            var task = client.DeleteAsync(uri);

			System.Diagnostics.Debug.WriteLine("Sending delete request for: " + uri);

            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
				string response = await task.Result.Content.ReadAsStringAsync();
				System.Diagnostics.Debug.WriteLine("Delete response received, string is: " + response);
				return task.Result.IsSuccessStatusCode && !response.Contains("\"success\":false");
            }
            else {
                System.Diagnostics.Debug.WriteLine("Response timeout");
				return false;
            }


            
        }

    }


}
