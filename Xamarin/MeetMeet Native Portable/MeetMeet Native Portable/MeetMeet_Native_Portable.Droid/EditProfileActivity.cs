
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

using Android.Gms.Common;
using ClientApp;
using Newtonsoft.Json;

namespace MeetMeet_Native_Portable.Droid
{
	public class OnEditProfileEventArgs: EventArgs 
	{
		private string mGender;
		private string mProfile;

		public string Gender 
		{
			get { return mGender; }
			set { mGender = value; }
		}

		public string Profile 
		{
			get { return mProfile; }
			set { mProfile = value; }
		}

		public OnEditProfileEventArgs(String gender, String profile) : base()
		{
			Gender = gender;
			Profile = profile;
			//Email = email;
		}
	}

	[Activity (Label = "EditProfileActivity")]			
	public class EditProfileActivity : Activity
	{
		private EditText mTxtGender;
		private EditText mTxtProfile;
		private Button mButtonEditProfileSave;

		private Profile userProfile;

		public EventHandler <OnEditProfileEventArgs> mOnEditProfileComplete;

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

		async void MButtonEditProfileSave_Click (object sender, EventArgs e)
		{
			if (mTxtGender.Text!= "" && mTxtProfile.Text!= "") 
			{
				userProfile.gender = mTxtGender.Text;
				userProfile.bio = mTxtProfile.Text;
				if (await Updater.UpdateObject (userProfile, MainActivity.serverURL, MainActivity.profile_ext)) 
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

