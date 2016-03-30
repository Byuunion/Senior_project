using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Android.Gms.Common;
using ClientApp;

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
		private Credentials credentials;

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
<<<<<<< HEAD
				//mOnEditProfileComplete.Invoke (
=======
				//mOnEditProfileComplete.Invoke (this, new OnEditProfileEventArgs (mTxtGender.Text, mTxtProfile.Text));
>>>>>>> origin/master
				//this.Dismiss ();
			}
		}

<<<<<<< HEAD
		/*private async Task<Boolean> TryToSaveProfile(string gender, string bio)
		{
			credentials = new Credentials(username);
			System.Diagnostics.Debug.WriteLine("Trying to save profile");
			var loggedIn = await credentials.doLogin(password, serverURL);

			if (loggedIn)
			{
				if (await Updater.UpdateObject(new { token = credentials.token, username = username, gcm_regid = gcm_token }, serverURL, gcm_regid_ext + "/" + username))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}*/
=======
		async void mOnEditProfileSaveComplete (Object sender, OnEditProfileEventArgs e)
		{
			string userGender = e.Gender;
			string userProfile = e.Profile;

			Profile myProfile = new Profile (credentials.username, userGender, userProfile, credentials.token);

			await Poster.PostObject (myProfile, serverURL);

		}
>>>>>>> origin/master
	}
		
}

