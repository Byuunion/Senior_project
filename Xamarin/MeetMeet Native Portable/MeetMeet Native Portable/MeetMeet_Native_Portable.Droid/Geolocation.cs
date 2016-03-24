using System;

namespace MeetMeet_Native_Portable.Droid
{
	public class Geolocation
	{
		private double latitude;
		private double longitude;
		private double latmin;
		private double latmax;
		private double longmin;
		private double longmax;

		public Geolocation (double latitude, double longitude)
		{
			this.latitude = latitude;
			this.longitude = longitude;
			double lat = toRadians(latitude);
			double lng = toRadians (longitude);
			double r = 0.1570; //constant angle 
			this.latmin = lat - r;
			this.latmax = lat + r;
			double deltaLong = Math.Asin (Math.Sin (r) / Math.Cos (lat));
			this.longmin = lng - deltaLong;
			this.longmax = lng + deltaLong;	
		}

		public double toRadians(double degrees)
		{
			return (Math.PI / 180.0) * degrees;
		}
	}
}