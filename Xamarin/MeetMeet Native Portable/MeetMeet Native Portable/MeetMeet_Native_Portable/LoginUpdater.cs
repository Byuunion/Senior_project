using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;


namespace MeetMeet_Native_Portable
{
    /// <summary>
    /// Updater for the user login, because we have to be able to get a token back from the request
    /// </summary>
    /// <typeparam name="T">The type of object expected back</typeparam>
    class LoginUpdater<T>
    {
        /// <summary>
        /// Send a login put request to the server
        /// </summary>
        /// <param name="obj">The object to put</param>
        /// <param name="url">The url to send the request to</param>
        /// <returns>The object returned by the server</returns>
        public static async Task<T> LoginUpdate(object obj, string url)
        {
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var uri = new Uri(url);

            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var task = client.PutAsync(uri, content);

            System.Diagnostics.Debug.WriteLine("Sending update request to: " + uri + " data: " + json);

            //Catch connection timeouts
            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                System.Diagnostics.Debug.WriteLine("Response received");
                string response = await task.Result.Content.ReadAsStringAsync();

                //Make sure the response was successful
                if (response.Contains("\"success\":false") || !task.Result.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Get was not successful");
                    return default(T);
                }
                else
                {
                    //Try to deserialize the object
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(response);
                    }
                    catch
                    {
                        return default(T);
                    }
                }
            }
            else {
                System.Diagnostics.Debug.WriteLine("Response timeout");
                return default(T);
            }
        }
    }
}
