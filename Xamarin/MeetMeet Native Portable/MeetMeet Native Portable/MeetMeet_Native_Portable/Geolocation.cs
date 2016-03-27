using System;

namespace MeetMeet_Native_Portable
{
	public class Geolocation : Updatable
	{
		public string username;
		public double latitude;
		public double longitude;
		public double latmin;
		public double latmax;
		public double longmin;
		public double longmax;

		public Geolocation (string username, double latitude, double longitude)
		{
			this.username = username;
			this.latitude = latitude;
			this.longitude = longitude;
			double lat = toRadians(latitude);
			double lng = toRadians (longitude);
			double r = 0.1570; //constant angle 
			this.latmin = toDegrees(lat - r);
			this.latmax = toDegrees(lat + r);
			double deltaLong = Math.Asin (Math.Sin (r) / Math.Cos (lat));
			this.longmin = toDegrees(lng - deltaLong);
			this.longmax = toDegrees(lng + deltaLong);	
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
	}
}