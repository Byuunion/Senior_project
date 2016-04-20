using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;


namespace MeetMeet_Native_Portable
{
	/// <summary>
	/// This class takes care of the details for sending a put request to our server
	/// </summary>
    public class Updater 
    {
		/// <summary>
		/// Updates the object.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="url">URL.</param>
		/// <param name="resource">Resource.</param>
        public static async Task<Boolean> UpdateObject(Object obj, string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var uri = new Uri(url);

            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var task = client.PutAsync(uri, content);

			System.Diagnostics.Debug.WriteLine ("Sending update request to: " + uri + " data: " + json);

            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                System.Diagnostics.Debug.WriteLine("Response received");
                System.Diagnostics.Debug.WriteLine(task.Status);
                return task.Result.IsSuccessStatusCode;
            }
            else {
                System.Diagnostics.Debug.WriteLine("Response timeout");
                return false;
            }
        }
    }
}
