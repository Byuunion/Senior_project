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

        public Credentials(string username)
        {
            this.mUsername = username;
            this.mToken = default(String);
        }

        public async Task<Boolean> doLogin(string password, string url)
        {
            var resource = username + "/" + password;
            var tempToken = await Getter<Token>.GetObjectNotFromList(resource, url);

            mToken = tempToken.token;

            if (token != default(String))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Boolean> doSignUp(string password, string url)
        {
            if(await Poster.PostObject(new Info(username, password), url))
            {
                return await doLogin(password, url + "/");
            }

            return false;
        }

    }

    public class Token
    {
        public string token { get; set; }
    }


    public class Info : Postable
    {
        public string password { get; set; }
        public string username { get; set; }

        public Info(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public string GetName()
        {
            return "";
        }
    }
}