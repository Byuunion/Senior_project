using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace MeetMeet_Native_Portable
{
	/// <summary>
	/// This class takes care of the details for sending a delete request to our server
	/// </summary>
    public class Deleter
    {

		/// <summary>
		/// Sends a delete request to the server to delete the information at the given extension
		/// </summary>
		/// <returns>Whether or not the deletion was successful</returns>
		/// <param name="url">The URL of the resource to delete</param>
		public static async Task<Boolean> DeleteObject(string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

			var uri = new Uri(url);
            var task = client.DeleteAsync(uri);

			System.Diagnostics.Debug.WriteLine("Sending delete request for: " + uri);

			// Catches time outs
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
