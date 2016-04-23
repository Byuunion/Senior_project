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

using Java.Util;

using Android.Gms.Common;
using ClientApp;
using Newtonsoft.Json;
using MeetMeet_Native_Portable;


namespace MeetMeet_Native_Portable.Droid
{

	/// <summary>
	/// Will provide functionality for main page.
	/// </summary>
	[Activity (Label = "MeetMeet", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		// Main Menu Items
		private Button mButtonSignUp;
		private Button mButtonSignIn;
        
		// User data
		public string userNameSignIn;
		public string userEmailSignIn;
		public string userPasswordSignIn;

		public string userNameSignUp;
		public string userEmailSignUp;
		public string userPasswordSignUp;
		public static string serverURL = URLs.serverURL;
		public static string login_ext = URLs.login_ext;
		public static string profile_ext = URLs.profile_ext;
		public static string location_ext = URLs.location_ext;
		public static string gcm_regid_ext = URLs.gcm_regid_ext;
		public static string group_message = URLs.group_message;
		public static string single_message = URLs.single_message;
		public static string pos_rating = URLs.pos_rating;
		public static string neg_rating = URLs.neg_rating;
		public static string blacklist_user = URLs.blacklist_user;
		public static string group_invite = URLs.group_invite;
		public static string user_group = URLs.user_group;
		public static string username;
		public static string user_token;
		public static string gcm_token;
		public static HashMap references;

		public static Credentials credentials;


		//var to check if play services work
		TextView msgText;

		/// <summary>
		/// Creates the event for main activity.
		/// Sets layout to main, references the main page button,
		/// and signIn/signOut button clicks.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			references = new HashMap ();

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//References for Main Menu Items
			mButtonSignUp = FindViewById<Button> (Resource.Id.SignUpButton);
			mButtonSignIn = FindViewById<Button> (Resource.Id.SignInButton);
            
			//Click Events

			//Sign Up Click
			mButtonSignUp.Click += mButtonSignUp_Click;

			//Sign in Click opens 
			mButtonSignIn.Click += MButtonSignIn_Click;

			msgText = FindViewById<TextView> (Resource.Id.msgText);

			if (IsPlayServicesAvailable ()) {
				var intent = new Intent (this, typeof(RegistrationIntentService));
				StartService (intent);
			}
		}

        /// <summary>
        /// Determines whether this instance is play services available.
        /// </summary>
        /// <returns><c>true</c> if this instance is play services available; otherwise, <c>false</c>.</returns>
        public bool IsPlayServicesAvailable ()
		{
			int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable (this);
			if (resultCode != ConnectionResult.Success) {
				if (GoogleApiAvailability.Instance.IsUserResolvableError (resultCode))
					msgText.Text = GoogleApiAvailability.Instance.GetErrorString (resultCode);
				else {
					msgText.Text = "Sorry, this device is not supported";
					Finish ();
				}
				return false;
			} else {
				msgText.Text = "Google Play Services is available.";
				return true;
			}
		}
			
		/// <summary>
		/// Starts Sign in dialog fragment via SignIn() when clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void MButtonSignIn_Click (object sender, EventArgs e)
		{
			//Pull up dialog
			FragmentTransaction transaction = FragmentManager.BeginTransaction ();
			SignIn signInDialog = new SignIn ();
			signInDialog.Show (transaction, "Dialog Fragment");

			// Subscribing to Signin Event

			signInDialog.mOnSignInComplete += signInDialog_mOnSignInComplete;

		}

		/// <summary>
		///  Retrieves data from Sign in activity and attempts to sign in user 
		///  to the database. If user has no profile it will start EditProfile Activity
		///  If user has a profile, HomeActivity will be started
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void signInDialog_mOnSignInComplete (Object sender, OnSignInEventArgs e)
		{
			// Retrieves data from SignIn Activity
			userNameSignIn = e.Username;
			userPasswordSignIn = e.Password;

			// attempts to log in. If user has no profile. User is notified and EditProfileActivity is started
			// and users profile object is passed.
			// Else statement occurs if user has a profile. HomeActivity is started and profile object is passed.
			if (await TryToLogin (userNameSignIn, userPasswordSignIn)) {
				var userProfile = await Getter<Profile>.GetObject (serverURL + profile_ext + "/" + userNameSignIn);
				if (userProfile == default(Profile)) {
					Toast.MakeText (this, "You don't have a profile to show", ToastLength.Short).Show ();
					string userGender = null;
					string userBio = null;
					Profile myProfile = new Profile (credentials.username, userGender, userBio, credentials.token);
					if (await Poster.PostObject (myProfile, serverURL + profile_ext)) {
						// pass profile object to EditProfileActivity
						Intent intent = new Intent (this, typeof(EditProfileActivity));
						var serializedProfile = JsonConvert.SerializeObject (myProfile);
						intent.PutExtra ("UserProfile", serializedProfile);
						StartActivity (intent);
					}
				} else {
					username = credentials.username;
					user_token = credentials.token;
					userProfile.token = credentials.token;
					//pass profile object to HomeActivity
					Intent intent = new Intent (this, typeof(HomeActivity));
					var serializedProfile = JsonConvert.SerializeObject (userProfile);
					intent.PutExtra ("UserProfile", serializedProfile);
					StartActivity (intent);
				}
			} else
				Toast.MakeText (this, "Login Unsuccessful ", ToastLength.Short).Show ();
		}

		// Sign Up Click

		/// <summary>
		/// Starts Sign in dialog fragment via SignIn() when clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void mButtonSignUp_Click (object sender, EventArgs e)
		{

			//Pull up dialog
			FragmentTransaction transaction = FragmentManager.BeginTransaction ();
			SignUp signUpDialog = new SignUp ();
			signUpDialog.Show (transaction, "Dialog Fragment");

			//Subscribing to Signup Event
			signUpDialog.mOnSignUpComplete += signUpDialog_mOnSignUpComplete;
		}

		/// <summary>
		/// On SignUp activity completion, this will attempt to sign up user to the database.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void signUpDialog_mOnSignUpComplete (Object sender, OnSignUpEventArgs e)
		{
			// Retrieves data from SignUp activity
			userNameSignUp = e.UserName;
			userPasswordSignUp = e.Password;

			/// Checks if username is taken. If taken, user is notified. Otherwise a new profile object is created.
			/// User is then taken to EditProfileActivity.
			if (await TryToSignUp (userNameSignUp, userPasswordSignUp)) {
				username = credentials.username;
				user_token = credentials.token;
				string userGender = null;
				string userBio = null;
				Profile myProfile = new Profile (credentials.username, userGender, userBio, credentials.token);
				if (await Poster.PostObject (myProfile, serverURL + profile_ext)) {
					// pass profile object to EditProfileActivity
					Intent intent = new Intent (this, typeof(EditProfileActivity));
					var serializedProfile = JsonConvert.SerializeObject (myProfile);
					intent.PutExtra ("UserProfile", serializedProfile);
					StartActivity (intent);
				}
			} else
				Toast.MakeText (this, "Signup Unsuccessful ", ToastLength.Short).Show ();
		}

		// Part of thread simulation

		/// <summary>
		/// Acts like A request.
		/// </summary>
		private async void ActLikeARequest ()
		{
			await TryToSignUp ("TestGCMUSER6", "password");

		}

		/// <summary>
		/// This method takes care of the login process
		/// </summary>
		/// <param name="username"> the user's username, gotten from the gui</param>
		/// <param name="password"> the user's password, gotten from the gui</param>
		/// <returns> whether or not the user was successfully logged in</returns>
		private async Task<Boolean> TryToLogin (string username, string password)
		{
			try {
				credentials = new Credentials (username);
				System.Diagnostics.Debug.WriteLine ("Trying to log in");
				bool loggedIn = await credentials.doLogin (password, serverURL);

				System.Diagnostics.Debug.WriteLine ("Finished doLogin, token is " + credentials.token);
				if (loggedIn) {
					return await Updater.UpdateObject (new { token = credentials.token, username = username, gcm_regid = gcm_token }, serverURL + gcm_regid_ext);
				} else {
					return false;
				}
			} catch {
				return false;
			}
		}

		/// <summary>
		/// Do the sign up procedure
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		private async Task<Boolean> TryToSignUp (string username, string password)
		{
			credentials = new Credentials (username);
			System.Diagnostics.Debug.WriteLine ("Trying to sign up");
			try {
				var loggedIn = await credentials.doSignUp (password, serverURL);

				if (loggedIn) {
					return await Poster.PostObject (new { token = credentials.token, username = username, gcm_regid = gcm_token }, serverURL + gcm_regid_ext);
				} else {
					return false;
				}
			} catch {
				return false;
			}
		}
	}
}


