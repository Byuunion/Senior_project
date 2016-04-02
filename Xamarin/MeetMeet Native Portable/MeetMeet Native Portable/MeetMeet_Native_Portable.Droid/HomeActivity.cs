using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Newtonsoft.Json;
using Android.Locations;
using Android.Content;


namespace MeetMeet_Native_Portable.Droid
{
	/// <summary>
	/// Will provide functionality for home page.
	/// </summary>
	[Activity(Label = "Home", Icon = "@drawable/icon")]
	public class HomeActivity : Activity, ILocationListener
	{
		private Profile userProfile;
		private LocationManager locationManager;
		private String provider;

		// Main Menu Items
		private Button mButtonNearbyUsers;
		private Button mButtonUpdateLocation;
		private Button mButtonEditProfile;

		// Show username at top to show Profile was passed correctly
		private TextView mTextViewUsername;

		/// <summary>
		/// Creates the event for the home activity.
		/// Sets layout to home_page, references the Find Nearby Users,
		/// Get Location and Edit Profile button clicks.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set context to this activity to be passed to NearbyUsersFragment
			Config.context = this;

			// Get profile object from sign-in
			var MyJsonString = Intent.GetStringExtra("UserProfile");
			userProfile = JsonConvert.DeserializeObject<Profile>(MyJsonString);

			// Set our view from the home_page resource
			SetContentView (Resource.Layout.home_page);

			// References for Home Menu Items
			mButtonNearbyUsers = FindViewById<Button> (Resource.Id.NearbyUsersButton);
			mButtonUpdateLocation = FindViewById<Button> (Resource.Id.SetLocationButton);
			mButtonEditProfile = FindViewById<Button> (Resource.Id.EditProfileButton);
			mTextViewUsername = FindViewById<TextView> (Resource.Id.UsernameTextView);

			// Set username text
			mTextViewUsername.Text = userProfile.username;

			//Click Events

			// Find Nearby Users Click
			mButtonNearbyUsers.Click += delegate {
				StartActivity (typeof(ProfileMainActivity));
			};

			// Set Current Location Click 
			mButtonUpdateLocation.Click += mButtonSetLocation_Click;		

			// Edit Profile Click
			mButtonEditProfile.Click += delegate {
				StartActivity (typeof(EditProfileActivity));
			};

			// Get the location manager
			locationManager = (LocationManager) GetSystemService(Context.LocationService);

			// Define the criteria how to select the location provider -> use default
			Criteria criteria = new Criteria();
			criteria.Accuracy = Accuracy.Fine;
			provider = locationManager.GetBestProvider(criteria, true);
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

		private void mButtonSetLocation_Click (object sender, EventArgs e)
		{
			Location location = locationManager.GetLastKnownLocation(provider);
			Geolocation currentLocation = new Geolocation (userProfile.username,location.Latitude, location.Longitude);
			//Geolocation currentLocation = new Geolocation (userProfile.username, 39.75259742, -75.21786803);
			//currentLocation.UpdateGeolocation ();
			currentLocation.UpdateGeolocation2 (userProfile.token);

		}
	}
}

 