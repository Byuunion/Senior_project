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
			var r = 6371; // Earth's radius in km
			//var r = 3960; // Earth's radius in mi

			var lat1 = 38.898556;// Latiude;
			var long1 = -77.037852;
			var latRad1 = toRadians(lat1);// toRadians(Latiude);

			var lat2 = 38.897147;// pull from database with username
			var long2 = -77.043934;// pull from database with username
			var latRad2 = toRadians(lat2);

			var	latitudeDiff = toRadians(lat2 - lat1);
			var longitudeDiff = toRadians(long2 - long1);

			var a = Math.Sin(latitudeDiff/2.0) * Math.Sin(latitudeDiff/2.0) +
				Math.Cos(latRad1) * Math.Cos(latRad2) *
				Math.Sin(longitudeDiff/2.0) * Math.Sin(longitudeDiff/2.0);
			
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));

			var d = r * c;
		}
	}
}

