using System;

using Android.App;
using Android.Widget;
using Android.OS;



namespace MeetMeet_Native_Portable.Droid
{

	/// <summary>
	/// Will provide functionality for home page.
	/// </summary>
	[Activity(Label = "Home", Icon = "@drawable/icon")]
	public class HomeActivity : Activity
	{

		// Main Menu Items
		private Button mButtonNearbyUsers;
		private Button mButtonGetLocation;
		private Button mButtonEditProfile;


		/// <summary>
		/// Creates the event for the home activity.
		/// Sets layout to home_page, references the Find Nearby Users,
		/// Get Location and Edit Profile button clicks.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate(Bundle bundle)
		{

			base.OnCreate (bundle);
			Config.context = this;

			// Set our view from the home_page resource
			SetContentView (Resource.Layout.home_page);

			//References for Main Menu Items
			mButtonNearbyUsers = FindViewById<Button> (Resource.Id.NearbyUsersButton);
			mButtonGetLocation = FindViewById<Button> (Resource.Id.GetLocationButton);
			mButtonEditProfile = FindViewById<Button> (Resource.Id.EditProfileButton);

			//Click Events

			// Find Nearby Users Click
			mButtonNearbyUsers.Click += delegate {
				StartActivity (typeof(ProfileMainActivity));
			};

			// Get Location Click 
			mButtonGetLocation.Click += delegate {
				StartActivity (typeof(LocationActivity));
			};		

			// Edit Profile Click
			mButtonEditProfile.Click += delegate {
				StartActivity (typeof(ProfileMainActivity));
			};
		}
			
	}
}

