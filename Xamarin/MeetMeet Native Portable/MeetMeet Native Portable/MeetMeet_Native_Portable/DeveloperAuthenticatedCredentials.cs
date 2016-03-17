using Amazon.CognitoIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using ThirdParty.Json.LitJson;

namespace MeetMeet_Native_Portable
{
    public class DeveloperAuthenticatedCredentials : CognitoAWSCredentials
    {
        /**
        *
        **/
        private const string URL = "";
        private const string PROVIDER_NAME = "";
        private const string IDENTITY_POOL_ID = "";
        /**
        *
        **/


        private static Amazon.RegionEndpoint CognitoRegion = Amazon.RegionEndpoint.USEast1;
        private string Username;

        public DeveloperAuthenticatedCredentials(string username)
            : base(IDENTITY_POOL_ID, CognitoRegion)
        {
            this.Username = username;
        }

        public override async System.Threading.Tasks.Task<CognitoAWSCredentials.IdentityState> RefreshIdentityAsync()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(string.Format(URL, this.Username));
            var content = await response.Content.ReadAsStringAsync();
            JsonData json = JsonMapper.ToObject(content);

            //The backend has to send us back an Identity and a OpenID token
            string identityId = json["IdentityId"].ToString();
            string token = json["Token"].ToString();

            var idState = new IdentityState(identityId, PROVIDER_NAME, token, false);

            response.Dispose();
            client.Dispose();


            return idState;
        }
    


    }
}
