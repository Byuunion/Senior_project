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

        public static async Task<T> GetObject(T obj, string url)
        {
            HttpClient client;
            T returnData = default(T);
            var uri = new Uri(string.Format(url, obj.GetGetName()));

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("Response was not successful");
                var content = await response.Content.ReadAsStringAsync();
                returnData = JsonConvert.DeserializeObject<List<T>>(content).ElementAt(0);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Response was not successful");
            }

            return returnData;
        }


    }
}
