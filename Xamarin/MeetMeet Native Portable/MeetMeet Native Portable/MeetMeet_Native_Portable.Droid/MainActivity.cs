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


namespace MeetMeet_Native_Portable.Droid
{

	/// <summary>
	/// Will provide functionality for main page.
	/// </summary>
    [Activity(Label = "MeetMeet", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
    {

		// Main Menu Items
		private Button mButtonSignUp;
		private Button mButtonSignIn;
		//private ProgressBar mProgressBar;
		private Button mLocationButton;
		private Button mProfileListSample;

		// User data
		public string userNameSignIn;
		public string userEmailSignIn;
		public string userPasswordSignIn;

		public string userNameSignUp;
		public string userEmailSignUp;
		public string userPasswordSignUp;
        public static string serverURL = "http://52.91.212.179:8800/";
        public static string login_ext = "user/login";
        public static string profile_ext = "user/profile";
        public static string location_ext = "user/profile/location";
        public static string gcm_regid_ext = "user/gcmregid";
        public static string username;
        public static string user_token;
        public static string gcm_token;

        private Credentials credentials;
        

        //var to check if play services work
        TextView msgText;

        /// <summary>
        /// Creates the event for main activity.
        /// Sets layout to main, references the main page button,
        /// and signIn/signOut button clicks.
        /// </summary>
        /// <param name="bundle">Bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
           
			base.OnCreate(bundle);
			Config.context = this;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

			//References for Main Menu Items
			mButtonSignUp = FindViewById<Button> (Resource.Id.SignUpButton);
			mButtonSignIn = FindViewById<Button> (Resource.Id.SignInButton);
			//mProgressBar = FindViewById<ProgressBar> (Resource.Id.progressBar1);
			mLocationButton = FindViewById<Button> (Resource.Id.TestLocationButton);
			mProfileListSample = FindViewById<Button> (Resource.Id.TestProfileList);

			//Click Events

			//Sign Up Click
			mButtonSignUp.Click += mButtonSignUp_Click;

			//Sign in Click opens 
			mButtonSignIn.Click += MButtonSignIn_Click;

			mLocationButton.Click += delegate {
				StartActivity (typeof(LocationActivity));
			};

			// ProfileListSample Click
			mProfileListSample.Click += delegate {
				StartActivity (typeof(ProfileMainActivity));
			};

            msgText = FindViewById<TextView>(Resource.Id.msgText);

            if (IsPlayServicesAvailable())
            {
                var intent = new Intent(this, typeof(RegistrationIntentService));
                StartService(intent);
            }

        }

        //checks to make sure google play services are running
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
				

		// Sign In Click

		/// <summary>
		/// Starts Sign in dialog fragment via SignIn() when clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
        void MButtonSignIn_Click (object sender, EventArgs e)
        {
			//Pull up dialog
			FragmentTransaction transaction = FragmentManager.BeginTransaction();
			SignIn signInDialog = new SignIn ();
			signInDialog.Show (transaction, "Dialog Fragment");

			// Subscribing to Signin Event

			signInDialog.mOnSignInComplete += signInDialog_mOnSignInComplete;


        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void signInDialog_mOnSignInComplete(Object sender, OnSignInEventArgs e)
		{
			// here we send request to server
			// just simulating here
			userNameSignIn = e.Username;
			userPasswordSignIn = e.Password;
			Thread thread = new Thread(ActLikeARequest);
			thread.Start ();
			//string userPassword = e.Password;

			//Post to server code. Will need two outcomes if can't log in or if successful

			//Success

			//Failure
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
			FragmentTransaction transaction = FragmentManager.BeginTransaction();
			SignUp signUpDialog = new SignUp();
			signUpDialog.Show(transaction, "Dialog Fragment");

			//Subscribing to Signup Event

			signUpDialog.mOnSignUpComplete += signUpDialog_mOnSignUpComplete;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void signUpDialog_mOnSignUpComplete(Object sender, OnSignUpEventArgs e)
		{
			// here we send request to server
			// just simulating here
			//Thread thread = new Thread(ActLikeARequest);
			//thread.Start ();
			//string userPassword = e.Password;

			userNameSignUp = e.UserName;
			userEmailSignUp = e.Email;
			userPasswordSignUp = e.Password;

			if (await TryToSignUp (userNameSignUp, userPasswordSignUp))
			{
				StartActivity(typeof(EditProfileActivity));
			} 
			await TryToSignUp(userNameSignUp, userPasswordSignUp);

			// Post to server code

			//Success

			//Failure
		}

		// Part of thread simulation

		/// <summary>
		/// Acts like A request.
		/// </summary>
		private async void ActLikeARequest()
		{
            await TryToSignUp("TestGCMUSER6", "password");
            
            /*
            Credentials test = new Credentials("houzec8");

            var loggedIn = await test.doLogin("passsdflkj", serverURL);

            if (loggedIn)
            {
                System.Diagnostics.Debug.WriteLine("Successfully signed in, token is: " + test.token);
                var testProfile = await Getter<Profile>.GetObject(test.username, serverURL + profile_ext + "/");
                testProfile.token = test.token;

                if (testProfile != default(Profile))
                {
                    testProfile.current_lat = 24;
                    testProfile.current_long = 31;
                    await Updater.UpdateObject(testProfile, serverURL + location_ext + "/");
                }
            }
            */

            /*
            Credentials test = new Credentials("test12");

            var loggedIn = await test.doSignUp("thisIsAPassword", serverURL + login);

            //var loggedIn = await test.doLogin("hunter2", serverURL + login);

            if (loggedIn)
            {
                System.Diagnostics.Debug.WriteLine("Successfully signed up, token is: " + test.token);

                Profile testProfile = new Profile("test12", "male", "This is a super amazing awesome bio", test.token);

                var posted = await Poster.PostObject(testProfile, serverURL + profile);

                if (posted)
                {
                    var response = await Getter<Profile>.GetObject(testProfile.username, serverURL + profile + "/");

                    if(response != default(Profile))
                    {
                        System.Diagnostics.Debug.WriteLine("Response received bio: {0} username: {1}", response.bio, response.username);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Could not get profile");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Could not post profile");
                }

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Could not sign up or log in");
            }
            */

            //RunOnUiThread (() => {mProgressBar.Visibility = ViewStates.Invisible; });
        }

        /// <summary>
        /// This method takes care of the login process
        /// </summary>
        /// <param name="username"> the user's username, gotten from the gui</param>
        /// <param name="password"> the user's password, gotten from the gui</param>
        /// <returns> whether or not the user was successfully logged in</returns>
        private async Task<Boolean> TryToLogin(string username, string password)
        {
            credentials = new Credentials(username);
            System.Diagnostics.Debug.WriteLine("Trying to log in");
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
        }

        /// <summary>
        /// Do the sign up procedure
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<Boolean> TryToSignUp(string username, string password)
        {
            credentials = new Credentials(username);
            System.Diagnostics.Debug.WriteLine("Trying to log in");
            var loggedIn = await credentials.doSignUp(password, serverURL);

            if (loggedIn)
            {
                if (await Poster.PostObject(new { token = credentials.token, username = username, gcm_regid = gcm_token }, serverURL + gcm_regid_ext))
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
        }
    }
}


