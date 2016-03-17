using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetMeet_Native_Portable
{

    class Login
    {
        private DeveloperAuthenticatedCredentials credentials;
        private bool LoggedIn;
        public async Task<bool> DoLogin(String username, String password)
        {
            LoggedIn = false;
            //Login using our own creditials
            LoggedIn = await Poster.PostObject(new CustomCredentials(username, password), "our url");
            

            //Get the token from the Cognito
            credentials = new DeveloperAuthenticatedCredentials(username);
            var IdentityState = credentials.RefreshIdentityAsync();

            if (IdentityState.IsCompleted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



    }
}
