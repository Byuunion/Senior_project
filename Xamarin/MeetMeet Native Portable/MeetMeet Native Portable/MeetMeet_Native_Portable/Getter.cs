using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace MeetMeet_Native_Portable
{
    public class Getter<T> where T : Gettable
    {

        private static async Task<string> GetAbstract(T obj, string url)
        {
            HttpClient client;
            string content = null;
            var uri = new Uri(string.Format(url, obj.GetGetName()));

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("Response was successful");
                string responseString = await response.Content.ReadAsStringAsync();
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Response was not successful");
            }

            return content;
        }

        public static async Task<List<T>> GetObjectList(T obj, string url)
        {
            string content = await GetAbstract(obj, url);
            return JsonConvert.DeserializeObject<List<T>>(content);
        }

        public static async Task<T> GetObject(T obj, string url)
        {
            string content = await GetAbstract(obj, url);
            return JsonConvert.DeserializeObject<List<T>>(content).ElementAt(0);
        }
    }
}
