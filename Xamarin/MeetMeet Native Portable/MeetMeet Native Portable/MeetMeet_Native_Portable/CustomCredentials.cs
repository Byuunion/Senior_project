using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetMeet_Native_Portable
{
    class CustomCredentials : Postable
    {
        //Not really sure how we're supposed to communicate passwords.
        //Looked at a bunch of stuff, but it seems like communicating over HTTP is (at least part of) the solution
        public string username, password;

        public CustomCredentials(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        string Postable.getName()
        {
            return username;
        }
    }
}
