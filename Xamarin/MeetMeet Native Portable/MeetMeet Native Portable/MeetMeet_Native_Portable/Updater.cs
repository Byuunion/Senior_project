using System;
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
		/// This method sends a put request to the server to update the given object
		/// </summary>
		/// <returns>Whether or not the put request was successful</returns>
		/// <param name="obj">The object to update</param>
		/// <param name="url">The url to send the request to</param>
        public static async Task<Boolean> UpdateObject(Object obj, string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var uri = new Uri(url);

            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var task = client.PutAsync(uri, content);

			System.Diagnostics.Debug.WriteLine ("Sending update request to: " + uri + " data: " + json);

            //Catch connection timeouts
            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                string response = await task.Result.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("Response received");

                //Make sure the response was successful
                return task.Result.IsSuccessStatusCode && !response.Contains("\"success\":false");
            }
            else {
                System.Diagnostics.Debug.WriteLine("Response timeout");
                return false;
            }
        }
    }
}
