using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace MeetMeet_Native_Portable
{
	/// <summary>
	/// This class takes care of the details for sending a post request to our server
	/// </summary>
    public class Poster
    {
		/// <summary>
		/// Sends a post request to the server with the given object
		/// </summary>
		/// <returns>Whether or not the post was successful</returns>
		/// <param name="obj">The object to send to the server</param>
		/// <param name="url">The URL of the server</param>
        public static async Task<Boolean> PostObject(Object obj, string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var uri = new Uri(url);
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var task = client.PostAsync(uri, content);
            
			System.Diagnostics.Debug.WriteLine("Trying to post object: " + json + " to: " + uri);

            //Catch connection timeouts
            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                string response = await task.Result.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("Post response received. String is: " + response);

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
