using System;

namespace MeetMeet_Native_Portable
{
	public class Location : Postable
	{
		public double latiude;
		public double longitude;
		public string username;

		public Location (String username)
		{
			this.username = username;
		}

		public string getName()
		{
			return this.username;
		}

		public double Latiude 
		{
			get { return this.latiude; }
			set { this.latiude = value; }
		}

		public double Longitude 
		{
			get { return this.longitude; }
			set { this.longitude = value; }
		}

		public double toRadians(double degrees)
		{
			return (Math.PI / 180) * degrees;
		}

		public void getDistance(Postable userToMeet)
		{
			var user2 = userToMeet.getName();
			var r = 6371000; // Earth's radius in metres

			var lat1 = toRadians(40.7486);// toRadians(Latiude);
			var long1 = toRadians(-73.9864);// toRadians(Longitude);

			var lat2 = toRadians(40.7486);// pull from database with username
			var long2 = toRadians(-73.9664);// pull from database with username

			var	latitudeDiff = toRadians(lat2-lat1);
			var longitudeDiff = toRadians(long2-long1);

			var a = Math.Sin(latitudeDiff/2) * Math.Sin(latitudeDiff/2) +
				Math.Cos(lat1) * Math.Cos(lat2) *
				Math.Sin(longitudeDiff/2) * Math.Sin(longitudeDiff/2);
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));

			var d = r * c;
		}
	}
}

