using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace MeetMeet_Native_Portable
{
	/// <summary>
	/// This class contains the Geolocation of the currently signed in user.
	/// This class also contains methods for converting degrees/radians,
	/// sending a put request to our server to update the user's location,
	/// and sending a get request to our server to find nearby users.
	/// </summary>
	public class Geolocation 
	{
		public string username;
		public double latitude;
		public double longitude;
		public double minLat;
		public double maxLat;
		public double minLong;
		public double maxLong;
		private string serverURL = "http://52.91.212.179:8800/";
		private string profileExt = "user/profile";

		/// <summary>
		/// Initializes a new instance of the <see cref="MeetMeet_Native_Portable.Geolocation"/> class.
		/// </summary>
		/// <param name="username">The username of the person that this geolocation belongs to</param>
		/// <param name="latitude">The latitude set by the user's location services</param>
		/// <param name="longitude">The longitude set by the user's location services</param>
		public Geolocation (string username, double latitude, double longitude)
		{
			this.username = username;
			this.latitude = latitude;
			this.longitude = longitude;
			double lat = toRadians(latitude);
			double lng = toRadians (longitude);
			double r = 0.1570; //constant angle 
			this.minLat = toDegrees(lat - r);
			this.maxLat = toDegrees(lat + r);
			double deltaLong = Math.Asin (Math.Sin (r) / Math.Cos (lat));
			this.minLong = toDegrees(lng - deltaLong);
			this.maxLong = toDegrees(lng + deltaLong);
		}

		/// <summary>
		/// Converts degrees to radians
		/// </summary>
		/// <param name="degrees">The degrees to be converted</param>
		public double toRadians(double degrees)
		{
			return (Math.PI / 180.0) * degrees;
		}

		/// <summary>
		/// Converts radians to degree 
		/// </summary>
		/// <param name="radians">The radians to be converted</param>
		public double toDegrees(double radians)
		{
			return (180.0 / Math.PI) * radians;
		}

		/// <summary>
		/// Gets the username that this geoloaction belongs to 
		/// </summary>
		public string GetName()
		{
			return this.username;
		}
			
		/// <summary>
		/// Gets the profile object of the user that this geoloaction belongs to and sets
		/// the current latitude and longitude. A put request with the profile is sent to 
		/// our server to update the user's geolocation.
		/// </summary>
		/// <param name="token">The token given by the server for this user, for use when updating a profile</param>
		public async void UpdateGeolocation(String token)
		{
			var userProfile = await Getter<Profile>.GetObject(serverURL + profileExt + "/" + username);
			userProfile.current_lat = latitude;
			userProfile.current_long = longitude;
			userProfile.token = token;
			await Updater.UpdateObject (userProfile, serverURL + profileExt);
		}
			
		/// <summary>
		/// Sends a get request with the calculated min/max lat and longs with the user's 
		/// current geolocation to our server to find nearby users.
		/// </summary>
		public async Task<List<Profile>> GetNearbyUsers()
		{
			var resource = minLat + "/" + maxLat + "/" + minLong + "/" + maxLong + "/" + latitude + "/" + longitude;
			var nearbylist = await Getter<Profile>.GetObjectList(serverURL + "user/" + resource);

			return nearbylist;
		}
	}
}