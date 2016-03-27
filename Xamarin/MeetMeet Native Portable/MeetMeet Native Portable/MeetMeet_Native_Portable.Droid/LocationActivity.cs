
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;

namespace MeetMeet_Native_Portable.Droid
{
	[Activity (Label = "LocationActivity")]			
	public class LocationActivity : Activity, ILocationListener
	{
		private TextView userlatitudeField;
		private TextView userlongitudeField;
		private TextView serverlatitudeField;
		private TextView serverlongitudeField;
		private TextView distanceField;
		private LocationManager locationManager;
		private String provider;
		public static string serverURL = "http://52.91.212.179:8800/";
		public static string loginExt = "user/login";
		public static string profileExt = "user/profile";
		public static string locationExt = "user/profile/location";

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.location);
			userlatitudeField = (TextView) FindViewById(Resource.Id.TextView02);
			userlongitudeField = (TextView) FindViewById(Resource.Id.TextView04);
			serverlatitudeField = (TextView) FindViewById(Resource.Id.TextView06);
			serverlongitudeField = (TextView) FindViewById(Resource.Id.TextView08);
			distanceField = (TextView) FindViewById(Resource.Id.TextView10);

			// Get the location manager
			locationManager = (LocationManager) GetSystemService(Context.LocationService);
			// Define the criteria how to select the location provider -> use
			// default
			Criteria criteria = new Criteria();
			provider = locationManager.GetBestProvider(criteria, false);
			Location location = locationManager.GetLastKnownLocation(provider);

			string username = "kshea12";
			// Initialize the location fields
			if (location != null) {
				Geolocation loc = new Geolocation (username, location.Latitude, location.Longitude);
				UpdateGeolocation (loc);
				userlatitudeField.Text = Convert.ToString(loc.latitude);
				userlongitudeField.Text = Convert.ToString(loc.longitude);

			}
			else {
				userlatitudeField.Text = "Location not available";
				userlongitudeField.Text = "Location not available";
			}
		}

		/* Request updates at startup */
		protected override void OnResume() {
			base.OnResume();
			locationManager.RequestLocationUpdates(provider, 1000, 0, this);
		}

		/* Remove the locationlistener updates when Activity is paused */
		protected override void OnPause() {
			base.OnPause();
			locationManager.RemoveUpdates(this);
		}
			
		public void OnLocationChanged(Location location) {
			double lat = location.Latitude;
			double lng = location.Longitude;
			//userlatitudeField.Text = Convert.ToString(lat);
			//userlongitudeField.Text = Convert.ToString(lng);
		}
			
		public void OnStatusChanged(String provider, Availability status, Bundle extras) {
			// TODO Auto-generated method stub

		}
			
		public void OnProviderEnabled(String provider) {
			Toast.MakeText(this, "Enabled new provider " + provider,
				ToastLength.Short).Show();
		}
			
		public void OnProviderDisabled(String provider) {
			Toast.MakeText (this, "Disabled provider " + provider,
				ToastLength.Short).Show();
		}

		private async void UpdateGeolocation(Geolocation location)
		{
			Credentials test = new Credentials("kshea12");

			var loggedIn = await test.doLogin("kevin", serverURL + loginExt + "/");

			if (loggedIn)
			{
				System.Diagnostics.Debug.WriteLine("Successfully signed in, token is: " + test.token);
				var testProfile = await Getter<Profile>.GetObject(test.username, serverURL + profileExt + "/");
				testProfile.token = test.token;

				if(testProfile != default(Profile))
				{
					testProfile.current_lat = location.latitude;
					testProfile.current_long = location.longitude;
					await Updater.UpdateObject(testProfile, serverURL + locationExt + "/");
				}
			}
		}

		private async void GetLocationFromServer(Location location)
		{
			double lat1 = location.Latitude;
			double lng1 = location.Longitude;
			userlatitudeField.Text = Convert.ToString(lat1);
			userlongitudeField.Text = Convert.ToString(lng1);

			Profile test = new Profile("KevyKevvv", "sflkj", "dsf", "token");

			var test2 = await Getter<Profile>.GetObject(test.username, "http://52.91.212.179:8800/user/profile/{0}");
			if(test2 == default(Profile))
			{
				userlatitudeField.Text = "null";
				userlongitudeField.Text = "null";
			}
			else
			{
				double lat2 = test2.current_lat;
				double lng2 = test2.current_long;
				serverlatitudeField.Text = Convert.ToString(lat2);
				serverlongitudeField.Text = Convert.ToString(lng2);
				distanceField.Text = string.Format("{0:0.00}", GetDistance (lat1, lng1, lat2, lng2)) + " mi";


			}
		}

		public double toRadians(double degrees)
		{
			return (Math.PI / 180.0) * degrees;
		}

		public double GetDistance(double lat1, double long1, double lat2, double long2)
		{
			//var r = 6371.0; // Earth's radius in km
			var r = 3960.0; // Earth's radius in mi

			var latRad1 = toRadians(lat1);// toRadians(Latiude);
			var latRad2 = toRadians(lat2);

			var	latitudeDiff = toRadians(lat2 - lat1);
			var longitudeDiff = toRadians(long2 - long1);

			var a = Math.Sin(latitudeDiff/2.0) * Math.Sin(latitudeDiff/2.0) +
				Math.Cos(latRad1) * Math.Cos(latRad2) *
				Math.Sin(longitudeDiff/2.0) * Math.Sin(longitudeDiff/2.0);

			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));

			var d = r * c;

			return d;
		}

		public double[] MinMaxLat(double latitude)
		{
			double r = 0.1570; //constant angle 
			var lat = toRadians(latitude);

			double[] minMax = new double[2];
			minMax [0] = lat - r;
			minMax [1] = lat + r;

			return minMax;
		}

		public double[] minMaxLong(double latitude, double longitude)
		{
			double r = 0.1570; //constant angle 
			var lat = toRadians (latitude);
			var lng = toRadians (longitude);

			double deltaLong = Math.Asin (Math.Sin (r) / Math.Cos (lat));

			double[] minMax = new double[2];
			minMax [0] = lng - deltaLong;
			minMax [1] = lng + deltaLong;

			return minMax;
		}
	}
}

