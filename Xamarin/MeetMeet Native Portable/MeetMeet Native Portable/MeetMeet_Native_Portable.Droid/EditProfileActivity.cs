
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

		public EventHandler <OnEditProfileEventArgs> mOnEditProfileComplete;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.edit_profile);
			// Create your application here

			mTxtGender = FindViewById<EditText>(Resource.Id.edittextgender);
			mTxtProfile = FindViewById<EditText>(Resource.Id.edittextprofile);
			mButtonEditProfileSave = FindViewById<Button> (Resource.Id.btnEditProfileSave);

			//Save Click Event

			mButtonEditProfileSave.Click += MButtonEditProfileSave_Click;
		
		}

		async void MButtonEditProfileSave_Click (object sender, EventArgs e)
		{
			if (mTxtGender.Text!= "" && mTxtProfile.Text!= "") 
			{
				Profile testProf = new Profile (MainActivity.username, mTxtGender.Text, mTxtProfile.Text, MainActivity.user_token);
				if (await Updater.UpdateObject (testProf, MainActivity.serverURL, MainActivity.profile_ext)) 
				{
					//var userProfile = await Getter<Profile>.GetObject (MainActivity.username, MainActivity.serverURL + MainActivity.profile_ext + "/");
					// pass profile object to HomeActivity
					//Intent intent = new Intent(this, typeof(HomeActivity));
					//var MySerializedObject = JsonConvert.SerializeObject(userProfile);
					//intent.PutExtra("UserProfile", MySerializedObject);
					//StartActivity(intent);
					StartActivity(typeof(HomeActivity));
				}
			}
		}
	}
}

