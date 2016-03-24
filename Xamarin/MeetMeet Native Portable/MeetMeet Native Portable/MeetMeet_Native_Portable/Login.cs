using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetMeet_Native_Portable
{
    public class Login
    {
        private string mUsername;
        public string username { get { return mUsername; } }
        private string mToken;
        public string token { get { return mToken; } }

        public Login (string username)
        {
            this.mUsername = username;
            this.mToken = default(String);
        }

        public async Task<Boolean> doLogin(string password, string url)
        {
            var resource = "user/login/" + username + "/" + password;
            var tempToken = await Getter<Token>.GetObjectNotFromList(resource, url);

            mToken = tempToken.token;

            if(token != default(String))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }

    public class Token
    {
        public string token { get; set; }
    }

}
