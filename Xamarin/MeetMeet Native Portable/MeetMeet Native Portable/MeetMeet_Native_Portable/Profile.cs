namespace MeetMeet_Native_Portable 
{
	/// <summary>
	/// Container object for a user profile
	/// </summary>
    public class Profile 
    {
        public string username;
        public int positive_votes;
        public int negative_votes;
        public double current_lat;
        public double current_long;
        public string gender;
        public string bio;
        public string token;


		/// <summary>
		/// Initializes a new instance of the <see cref="MeetMeet_Native_Portable.Profile"/> class.
		/// </summary>
		/// <param name="username">The username of the person that this profile belongs to</param>
		/// <param name="gender">The gender set by the person that this profile belongs to</param>
		/// <param name="bio">The bio set by the person that this profile belongs to</param>
		/// <param name="token">The token given by the server for this user, for use when updating this profile</param>
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
    }
}