
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

using Newtonsoft.Json;

namespace MeetMeet_Native_Portable.Droid
{
	/// <summary>
	/// On edit profile event arguments.
	/// </summary>
	public class OnEditProfileEventArgs: EventArgs 
	{
		//
		private string mGender;
		private string mProfile;

		/// <summary>
		/// Gets or sets the gender.
		/// </summary>
		/// <value>The gender.</value>
		public string Gender 
		{
			get { return mGender; }
			set { mGender = value; }
		}

		/// <summary>
		/// Gets or sets the profile.
		/// </summary>
		/// <value>The profile.</value>
		public string Profile 
		{
			get { return mProfile; }
			set { mProfile = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MeetMeet_Native_Portable.Droid.OnEditProfileEventArgs"/> class.
		/// </summary>
		/// <param name="gender">Gender.</param>
		/// <param name="profile">Profile.</param>
		public OnEditProfileEventArgs(String gender, String profile) : base()
		{
			Gender = gender;
			Profile = profile;
		}
	}

	[Activity (Label = "EditProfileActivity")]			

	/// <summary>
	/// Edit profile activity.
	/// </summary>
	public class EditProfileActivity : Activity
	{
		private EditText mTxtGender;
		private EditText mTxtProfile;
		private Button mButtonEditProfileSave;

		private Profile userProfile;

		public EventHandler <OnEditProfileEventArgs> mOnEditProfileComplete;

		/// <summary>
		/// /
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		async protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the home_page resource
			SetContentView (Resource.Layout.edit_profile);

			// Get profile object from other activities
			var jsonString = Intent.GetStringExtra("UserProfile");
			userProfile = JsonConvert.DeserializeObject<Profile>(jsonString);

			//References for view items
			mTxtGender = FindViewById<EditText>(Resource.Id.edittextgender);
			mTxtProfile = FindViewById<EditText>(Resource.Id.edittextprofile);
			mButtonEditProfileSave = FindViewById<Button> (Resource.Id.btnEditProfileSave);

			if (userProfile.gender != null)
				mTxtGender.Text = userProfile.gender;
			if (userProfile.bio != null)
				mTxtProfile.Text = userProfile.bio;
			
			// Save Profile Click Event
			mButtonEditProfileSave.Click += MButtonEditProfileSave_Click;
		}

		/// <summary>
		/// Ms the button edit profile save click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void MButtonEditProfileSave_Click (object sender, EventArgs e)
		{
			if (mTxtGender.Text!= "" && mTxtProfile.Text!= "") 
			{
				userProfile.gender = mTxtGender.Text;
				userProfile.bio = mTxtProfile.Text;
				if (await Updater.UpdateObject (userProfile, MainActivity.serverURL + MainActivity.profile_ext)) 
				{
					// pass profile object to HomeActivity
					Intent intent = new Intent(this, typeof(HomeActivity));
					var serializedObject = JsonConvert.SerializeObject(userProfile);
					intent.PutExtra("UserProfile", serializedObject);
					StartActivity(intent);
				}
				else
					Toast.MakeText (this, "Profile Update Unsuccessful", ToastLength.Short).Show();
			}
		}
	}
}

