using System;

namespace MeetMeet_Native_Portable 
{
    public class Profile : Postable
    {
        public string username;
        public int positive_votes;
        public int negative_votes;
        public double current_lat;
        public double current_long;
        public string gender;
        public string bio;


        public Profile(string username, int positive_votes, int negative_votes, string gender)
        {
            this.username = username;
            this.gender = gender;
            this.positive_votes = positive_votes;
            this.negative_votes = positive_votes;
            this.current_lat = 0.0;
            this.current_long = 0.0;
            this.bio = "User has not set up a bio yet.";
        }

        public string GetName()
        {
            return this.username;
        }
    }
}