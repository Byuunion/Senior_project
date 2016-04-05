using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace MeetMeet_Native_Portable
{
    public class Getter<T>
    {

        private static async Task<string> GetAbstract(string resource, string url)
        {
            HttpClient client;
            string content = null;
            var uri = new Uri(url + resource);

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var response = await client.GetAsync(uri);

            System.Diagnostics.Debug.WriteLine("Response: " + await response.Content.ReadAsStringAsync());


            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("Get was successful, string is " + responseString);
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Get was not successful");
            }

            return content;
        }

        public static async Task<List<T>> GetObjectList(string resource, string url)
        {
            string content = await GetAbstract(resource, url);
            try
            {
                return JsonConvert.DeserializeObject<List<T>>(content);
            }
            catch(Exception e)
            {
                return new List<T>();
            } 
        }

        public static async Task<T> GetObject(string resource, string url)
        {
            string content = await GetAbstract(resource, url);

            //This line should not be necessary, it it the result of the server returning data directly from a MySQL query.
            //rather than returning a list with one item, the server should just send back the single item by itself
            try
            {
                   return JsonConvert.DeserializeObject<List<T>>(content).ElementAt(0);
            }
            catch
            {
                return default(T);
            }

            //In reality, the contents of this method should be replaced with the contents from "GetObjectNotFromList"
            //return JsonConvert.DeserializeObject<T>(content);
        }

        /**
        *
        *Note: This is a workaround method and should not be 
        *
        */
        public static async Task<T> GetObjectNotFromList(string resource, string url)
        {
            string content = await GetAbstract(resource, url);
            System.Diagnostics.Debug.WriteLine("String gotten from server " + content);
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch
            {
                return default(T);
            }
            
        }
    }
}
