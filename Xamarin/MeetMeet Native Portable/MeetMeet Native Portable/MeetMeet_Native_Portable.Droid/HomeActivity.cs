using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Newtonsoft.Json;



namespace MeetMeet_Native_Portable.Droid
{

	/// <summary>
	/// Will provide functionality for home page.
	/// </summary>
	[Activity(Label = "Home", Icon = "@drawable/icon")]
	public class HomeActivity : Activity
	{
		private Profile userProfile;

		// Main Menu Items
		private Button mButtonNearbyUsers;
		private Button mButtonGetLocation;
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
			mButtonGetLocation = FindViewById<Button> (Resource.Id.GetLocationButton);
			mButtonEditProfile = FindViewById<Button> (Resource.Id.EditProfileButton);
			mTextViewUsername = FindViewById<TextView> (Resource.Id.blankspace);

			// Set username text
			mTextViewUsername.Text = userProfile.username;

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
				StartActivity (typeof(EditProfileActivity));
			};
		}
			
	}
}

 