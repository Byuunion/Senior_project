
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
		private LocationManager locationManager;
		private String provider;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.location);
			userlatitudeField = (TextView) FindViewById(Resource.Id.TextView02);
			userlongitudeField = (TextView) FindViewById(Resource.Id.TextView04);
			serverlatitudeField = (TextView) FindViewById(Resource.Id.TextView06);
			serverlongitudeField = (TextView) FindViewById(Resource.Id.TextView08);

			// Get the location manager
			locationManager = (LocationManager) GetSystemService(Context.LocationService);
			// Define the criteria how to select the location provider -> use
			// default
			Criteria criteria = new Criteria();
			provider = locationManager.GetBestProvider(criteria, false);
			Location location = locationManager.GetLastKnownLocation(provider);

			// Initialize the location fields
			if (location != null) {
				//OnLocationChanged (location);
				double lat = location.Latitude;
				double lng = location.Longitude;
				userlatitudeField.Text = Convert.ToString(lat);
				userlongitudeField.Text = Convert.ToString(lng);
				GetLocationFromServer();
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

		private async void GetLocationFromServer()
		{
			Profile test = new Profile("KevyKevvv", null, null, null);
			// await Poster.PostObject(test, "http://52.91.212.179:8800/user/");

			var test2 = await Getter<Profile>.GetObject(test, "http://52.91.212.179:8800/user/profile/{0}");
			if(test2 == default(Profile))
			{
				userlatitudeField.Text = "null";
				userlongitudeField.Text = "null";
			}
			else
			{
				double lat = test2.current_lat;
				double lng = test2.current_long;
				serverlatitudeField.Text = Convert.ToString(lat);
				serverlongitudeField.Text = Convert.ToString(lng);
			}
		}
	}
}

