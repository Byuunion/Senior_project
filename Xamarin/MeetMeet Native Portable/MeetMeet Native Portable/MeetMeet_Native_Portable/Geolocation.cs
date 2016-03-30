using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace MeetMeet_Native_Portable
{
	public class Geolocation : Updatable
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
		private string locationExt = "user/profile/location";


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

		public double toRadians(double degrees)
		{
			return (Math.PI / 180.0) * degrees;
		}

		public double toDegrees(double radians)
		{
			return (180.0 / Math.PI) * radians;
		}

		public string GetName()
		{
			return this.username;
		}

		public async void UpdateGeolocation()
		{
			Credentials test = new Credentials(username);
			var loggedIn = await test.doLogin("kevin", serverURL);

			if (loggedIn)
			{
				System.Diagnostics.Debug.WriteLine("Successfully signed in, token is: " + test.token);
				var testProfile = await Getter<Profile>.GetObject(test.username, serverURL + profileExt + "/");
				testProfile.token = test.token;

				if(testProfile != default(Profile))
				{
					testProfile.current_lat = latitude;
					testProfile.current_long = longitude;
					await Updater.UpdateObject(testProfile, serverURL + locationExt + "/", testProfile.username);
				}
			}
		}

		public async Task<List<Geolocation>> GetNearbyUsers()
		{
			var resource = minLat + "/" + maxLat + "/" + minLong + "/" + maxLong + "/" + latitude + "/" + longitude;
			var nearbylist = await Getter<Geolocation>.GetObjectList(resource, serverURL + "user/");

			return nearbylist;
		}
	}
}