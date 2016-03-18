using System;

namespace MeetMeet_Native_Portable 
{
    public class Profile : Postable, Gettable
    {
        public string username;
        public string first_name;
        public string last_name;
        public int positive_votes;
        public int negative_votes;
        public int current_lat;
        public int current_long;
        public string gender;
        public string bio;


        public Profile(string username, string first_name, string last_name, string gender)
        {
            this.username = username;
            this.first_name = first_name;
            this.last_name = last_name;
            this.gender = gender;
            this.positive_votes = 0;
            this.negative_votes = 0;
            this.current_lat = 0;
            this.current_long = 0;
            this.bio = "User has not set up a bio yet.";
        }

        public string GetName()
        {
            return this.username;
        }

        public string GetGetName()
        {
            return this.username;
        }
    }
}