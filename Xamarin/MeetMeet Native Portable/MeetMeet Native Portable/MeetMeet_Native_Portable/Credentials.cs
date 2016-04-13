using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetMeet_Native_Portable
{
    public class Credentials
    {
        private string mUsername;
        public string username { get { return mUsername; } }
        private string mToken;
        public string token { get { return mToken; } }
        public static string login = "user/login";

        public Credentials(string username)
        {
            this.mUsername = username;
            this.mToken = default(String);
        }

        public async Task<Boolean> doLogin(string password, string url)
        {
            var resource = login + "/" +  username + "/" + password;
            System.Diagnostics.Debug.WriteLine("Resource trying to get " + resource);
            var tempToken = await Getter<Token>.GetObjectNotFromList(resource, url);

            if (tempToken != default(Token))
            {
                mToken = tempToken.token;
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Boolean> doSignUp(string password, string url)
        {
            if(await Poster.PostObject(new { username = this.username, password = password }, url + login))
            {
                System.Diagnostics.Debug.WriteLine("Successfully signed up user: " + this.username);
                return await doLogin(password, url);
            }

            return false;
        }

    }

    public class Token
    {
        public string token { get; set; }
    }

}