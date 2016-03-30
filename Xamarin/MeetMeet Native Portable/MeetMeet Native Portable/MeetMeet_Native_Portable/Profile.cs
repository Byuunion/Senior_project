using System;

namespace MeetMeet_Native_Portable 
{
    public class Profile : Updatable
    {
        public string username;
        public int positive_votes;
        public int negative_votes;
        public double current_lat;
        public double current_long;
        public string gender;
        public string bio;
        public string token;



        public Profile(string username, string gender, string bio, string token)
        {
            this.username = username;
            this.gender = gender;
            this.positive_votes = 0;
            this.negative_votes = 0;
            this.current_lat = 0.0;
            this.current_long = 0.0;
            this.bio = bio;
            this.token = token;
        }

        public string GetName()
        {
            return this.username;
        }


    }
}