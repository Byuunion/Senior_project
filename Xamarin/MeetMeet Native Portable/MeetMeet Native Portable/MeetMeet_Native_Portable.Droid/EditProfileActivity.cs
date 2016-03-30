
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

using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Android.Gms.Common;
using ClientApp;


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
		public static string serverURL = "http://52.91.212.179:8800/";
		private Credentials credentials;
		private EditText mTxtGender;
		private EditText mTxtProfile;
		private Button mButtonEditProfileSave;

		public EventHandler <OnEditProfileEventArgs> mOnEditProfileComplete;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.edit_profile);
			// Create your application here

			mTxtGender = FindViewById<EditText>(Resource.Id.edittextgender);
			mTxtProfile = FindViewById<EditText>(Resource.Id.edittextprofile);
			mButtonEditProfileSave = FindViewById<Button> (Resource.Id.btnEditProfileSave);

			//Save Click Event

			mButtonEditProfileSave.Click += MButtonEditProfileSave_Click;
		
		}

		void MButtonEditProfileSave_Click (object sender, EventArgs e)
		{
			if (mTxtGender.Text!= "" && mTxtProfile.Text!= "") 
			{
				//mOnEditProfileComplete.Invoke (this, new OnEditProfileEventArgs (mTxtGender.Text, mTxtProfile.Text));
				//this.Dismiss ();
			}
		}

		async void mOnEditProfileSaveComplete (Object sender, OnEditProfileEventArgs e)
		{
			string userGender = e.Gender;
			string userProfile = e.Profile;

			Profile myProfile = new Profile (credentials.username, userGender, userProfile, credentials.token);

			await Poster.PostObject (myProfile, serverURL);

		}
	}
		
}

