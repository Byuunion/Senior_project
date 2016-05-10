using System;
using System.Net;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Gms.Common;

using Java.Util;

using ClientApp;

using Newtonsoft.Json;


namespace MeetMeet_Native_Portable.Droid
{

	/// <summary>
	/// Will provide functionality for main page.
	/// </summary>
	[Activity (Label = "MeetMeet", MainLauncher = true, Icon = "@drawable/mmlogolarge")]
	public class MainActivity : Activity
	{
		// Main Menu Items
		private Button mButtonSignUp;
		private Button mButtonSignIn;

        //URLs and extensions for communicating with the server
		public static string serverURL = URLs.serverURL;
		public static string login_ext = URLs.login;
		public static string profile_ext = URLs.profile;
		public static string location_ext = URLs.location;
		public static string gcm_regid_ext = URLs.gcm_regid;
		public static string group_message = URLs.group_message;
		public static string single_message = URLs.single_message;
		public static string pos_rating = URLs.pos_rating;
		public static string neg_rating = URLs.neg_rating;
		public static string blacklist_user = URLs.blacklist_user;
		public static string group_invite = URLs.group_invite;
		public static string user_group = URLs.user_group;

        // User data
        public string userNameSignIn;
        public string userPasswordSignIn;

        public string userNameSignUp;
        public string userPasswordSignUp;

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
            //For testing purposes
            //Allows us to use a self signed server certificate
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
            //End  

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

            //Check to see if the app can use GCM 
			if (IsPlayServicesAvailable ())
            {
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
			if (resultCode != ConnectionResult.Success)
            {
				if (GoogleApiAvailability.Instance.IsUserResolvableError (resultCode))
					msgText.Text = GoogleApiAvailability.Instance.GetErrorString (resultCode);
				else
                {
					msgText.Text = "Sorry, this device is not supported";
					Finish ();
				}
				return false;
			}
            else
            {
				msgText.Text = "Google Play Services is available.";
				return true;
			}
		}

        /// <summary>
        /// Starts Sign in dialog fragment via SignIn() when clicked.
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
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
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
        public async void signInDialog_mOnSignInComplete (Object sender, OnSignInEventArgs e)
		{
			// Retrieves data from SignIn Activity
			userNameSignIn = e.Username;
			userPasswordSignIn = e.Password;

            // Attempts to log in. 
            //If user has no profile, they are notified and EditProfileActivity is started
			// and users profile object is passed.
			// Else statement occurs if user has a profile. HomeActivity is started and profile object is passed.
			if (await TryToLogin (userNameSignIn, userPasswordSignIn))
            {
				var userProfile = await Getter<Profile>.GetObject (serverURL + profile_ext + "/" + userNameSignIn);
				if (userProfile == default(Profile))
                {
					Toast.MakeText (this, "You don't have a profile to show", ToastLength.Short).Show ();
					string userGender = null;
					string userBio = null;
					Profile myProfile = new Profile (credentials.username, userGender, userBio, credentials.token);

					if (await Poster.PostObject (myProfile, serverURL + profile_ext))
                    {
						// pass profile object to EditProfileActivity
						Intent intent = new Intent (this, typeof(EditProfileActivity));
						var serializedProfile = JsonConvert.SerializeObject (myProfile);
						intent.PutExtra ("UserProfile", serializedProfile);
						StartActivity (intent);
					}
				}
                else
                {
                    //Set the variables associated with the user session
					username = credentials.username;
					user_token = credentials.token;
					userProfile.token = credentials.token;

					//Open the main menu and pass the profile object
					Intent intent = new Intent (this, typeof(HomeActivity));
					var serializedProfile = JsonConvert.SerializeObject (userProfile);
					intent.PutExtra ("UserProfile", serializedProfile);
					StartActivity (intent);
				}
			}
            else
				Toast.MakeText (this, "Login Unsuccessful ", ToastLength.Short).Show ();
		}


        /// <summary>
        /// Starts Sign in dialog fragment via SignIn() when clicked.
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
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
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
        async void signUpDialog_mOnSignUpComplete (Object sender, OnSignUpEventArgs e)
		{
			// Retrieves data from SignUp activity
			userNameSignUp = e.UserName;
			userPasswordSignUp = e.Password;

			/// Checks if username is taken. If taken, user is notified. Otherwise a new profile object is created.
			/// User is then taken to EditProfileActivity.
			if (await TryToSignUp (userNameSignUp, userPasswordSignUp))
            {
                //Create a default profile to start
				username = credentials.username;
				user_token = credentials.token;
				string userGender = null;
				string userBio = null;
				Profile myProfile = new Profile (credentials.username, userGender, userBio, credentials.token);

                //Send the temporary profile to the server
				if (await Poster.PostObject (myProfile, serverURL + profile_ext))
                {
					// pass profile object to EditProfileActivity, so the user can fill in their information
					Intent intent = new Intent (this, typeof(EditProfileActivity));
					var serializedProfile = JsonConvert.SerializeObject (myProfile);
					intent.PutExtra ("UserProfile", serializedProfile);
					StartActivity (intent);
				}
			}
            else
				Toast.MakeText (this, "Signup Unsuccessful ", ToastLength.Short).Show ();
		}

		/// <summary>
		/// This method takes care of the login process
		/// </summary>
		/// <param name="username"> the user's username, gotten from the gui</param>
		/// <param name="password"> the user's password, gotten from the gui</param>
		/// <returns> whether or not the user was successfully logged in</returns>
		private async Task<Boolean> TryToLogin (string username, string password)
		{
			try
            {
				credentials = new Credentials (username);

                //Try to log the user into the system
				if (await credentials.doLogin(password, serverURL + login_ext))
                {
                    //If the login was successful, update the user's GCM id
					return await Updater.UpdateObject (new { token = credentials.token, username = username, gcm_regid = gcm_token }, serverURL + gcm_regid_ext);
				}
                else
                {
					return false;
				}
			}
            catch
            {
				return false;
			}
		}

        /// <summary>
        /// Do the sign up procedure
        /// </summary>
        /// <param name="username"> the user's username, gotten from the gui</param>
        /// <param name="password"> the user's password, gotten from the gui</param>
        /// <returns> whether or not the user was successfully signed up</returns>
        private async Task<Boolean> TryToSignUp (string username, string password)
		{
			credentials = new Credentials (username);

			try
            {
                //Try to register the user with the system
				if (await credentials.doSignUp(password, serverURL + login_ext))
                {
                    //If registration was successful, send a request to create a new entry in the table with GCM ids
					return await Poster.PostObject (new { token = credentials.token, username = username, gcm_regid = gcm_token }, serverURL + gcm_regid_ext);
				}
                else
                {
					return false;
				}
			}
            catch
            {
				return false;
			}
		}
	}
}


