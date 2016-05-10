using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace MeetMeet_Native_Portable
{
	/// <summary>
	/// This class takes care of the details for sending a get request to our server
	/// </summary>
    public class Getter<T>
    {
		/// <summary>
		/// This method does the actual get request to the server, and returns the given string to be 
		/// processed in a specific way
		/// </summary>
		/// <returns>The raw string gotten from the server</returns>
		/// <param name="resource">The resource to get</param>
		/// <param name="url">The URL of the server</param>
        private static async Task<string> GetAbstract(string url)
        {
            HttpClient client;
            var uri = new Uri(url);

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

			System.Diagnostics.Debug.WriteLine("Sending get request for " + uri);

            var task = client.GetAsync(uri);

            //Catch connection timeouts
            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                string response = await task.Result.Content.ReadAsStringAsync();

                //Make sure the response was successful
                if (response.Contains("\"success\":false") || !task.Result.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Get was not successful");
                    return default(string);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Get for " + url + " was successful, string is " + response);
                    return response;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Response timeout");
                return default(string);
            }
        }

		/// <summary>
		/// Gets a list of objects from the server
		/// </summary>
		/// <returns>The list of objects returned by the server</returns>
		/// <param name="resource">The resource locator that the objects are stored at</param>
		/// <param name="url">The URL of the server</param>
        public static async Task<List<T>> GetObjectList(string url)
        {
            string content = await GetAbstract(url);
            try
            {
                return JsonConvert.DeserializeObject<List<T>>(content);
            }
            catch
            {
                return new List<T>();
            } 
        }

		/// <summary>
		/// Gets a single object from the server
		/// </summary>
		/// <returns>The object returned by the server</returns>
		/// <param name="resource">The resource locator that the object is stored at</param>
		/// <param name="url">URL.</param>
        public static async Task<T> GetObject(string url)
        {
            string content = await GetAbstract(url);

			if (content != default(string)) 
			{
				try 
				{
					// Because the server returns (almost) everything as a list, even if that 
					// list only contains one item 
					return JsonConvert.DeserializeObject<List<T>> (content).ElementAt (0);
				} 
				catch 
				{
					return default(T);
				}
			} 
			else 
			{
				return default(T);
			}
        }

        /// <summary>
        /// Gets a single object that is not returned as a list
        /// </summary>
        /// <returns>The object returned by the server</returns>
        /// <param name="resource">The resource locator that the object is stored at</param>
        /// <param name="url">The URL of the server</param>
        public static async Task<T> GetObjectNotFromList(string url)
        {
            string content = await GetAbstract(url);
            System.Diagnostics.Debug.WriteLine("String gotten from server " + content);
            if(content != default(string))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(content);
                }
                catch
                {
                    return default(T);
                }
            }
            else
            {
                return default(T);
            }
        }
    }
}
