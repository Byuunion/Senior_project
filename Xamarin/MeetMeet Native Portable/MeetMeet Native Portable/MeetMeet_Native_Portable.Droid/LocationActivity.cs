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

			string username = "a";
			// Initialize the location fields
			if (location != null) {
				Geolocation loc = new Geolocation (username, location.Latitude, location.Longitude);
				loc.PrototypeUpdateGeolocation ();

				userlatitudeField.Text = Convert.ToString(loc.latitude);
				userlongitudeField.Text = Convert.ToString(loc.longitude);
				//UpdateGeolocation (loc);
				//GetUsersNearby (loc);
				//serverlatitudeField.Text = loc.nearbyUsers;

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
			var loggedIn = await test.doLogin("kevin", serverURL);
			//Credentials test = new Credentials("dummy4");
			//var loggedIn = await test.doLogin("dummy4", serverURL);

			if (loggedIn)
			{
				System.Diagnostics.Debug.WriteLine("Successfully signed in, token is: " + test.token);
				var testProfile = await Getter<Profile>.GetObject(serverURL + profileExt + "/" + test.username);
				testProfile.token = test.token;

				if(testProfile != default(Profile))
				{
					testProfile.current_lat = 40.73061000;//location.latitude;
					testProfile.current_long = -73.93524200;//location.longitude;
					await Updater.UpdateObject(testProfile, serverURL + locationExt + "/" + testProfile.username);
				}
			}
		}

		private async void GetUsersNearby(Geolocation loc)
		{
			var resource = loc.minLat + "/" + loc.maxLat + "/" + loc.minLong + "/" + loc.maxLong + "/" + loc.latitude + "/" + loc.longitude;
			var nearbyUserList = await Getter<Geolocation>.GetObjectList(serverURL + "user/" + resource);
			String names = "";
			foreach (var user in nearbyUserList) {
				if (loc.username != user.username)
					names += user.username + " ";
			}
			serverlatitudeField.Text = names;

		}

		private async void GetLocationFromServer(Location location)
		{
			double lat = location.Latitude;
			double lng = location.Longitude;
			userlatitudeField.Text = Convert.ToString(lat);
			userlongitudeField.Text = Convert.ToString(lng);

			Profile test = new Profile("KevyKevvv", "sflkj", "dsf", "token");

			var test2 = await Getter<Profile>.GetObject(MainActivity.serverURL + MainActivity.profile_ext + test.username);
			if(test2 == default(Profile))
			{
				userlatitudeField.Text = "null";
				userlongitudeField.Text = "null";
			}
			else
			{
				Geolocation serverLocation = new Geolocation (test2.username, test2.current_lat, test2.current_long);
				serverlatitudeField.Text = Convert.ToString(serverLocation.latitude);
				serverlongitudeField.Text = Convert.ToString(serverLocation.longitude);
				distanceField.Text = string.Format("{0:0.00}", serverLocation.GetDistance (lat, lng, serverLocation.latitude, serverLocation.longitude)) + " mi";


			}
		}
	}
}

