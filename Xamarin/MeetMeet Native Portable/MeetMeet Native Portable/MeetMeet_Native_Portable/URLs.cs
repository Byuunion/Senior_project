using System;

namespace MeetMeet_Native_Portable
{
	/// <summary>
	/// Contains the URLs used by our application
	/// </summary>
	public static class URLs
	{
		public static string serverURL = "http://52.91.212.179:8800/";
		public static string login_ext = "user/login";
		public static string profile_ext = "user/profile";
		public static string location_ext = "user/profile/location";
		public static string gcm_regid_ext = "user/gcmregid";
		public static string group_message = "user/group/message";
		public static string single_message = "user/message";
		public static string pos_rating = "user/pos_rating";
		public static string neg_rating = "user/neg_rating";
		public static string blacklist_user = "user/blacklist";
		public static string group_invite = "user/group/invite";
		public static string user_group = "user/group";
	}
}

