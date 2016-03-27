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
		private ProgressBar mProgressBar;
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
        public static string loginExt = "user/login";
        public static string profileExt = "user/profile";
        public static string locationExt = "user/profile/location";

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

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

			//References for Main Menu Items
			mButtonSignUp = FindViewById<Button> (Resource.Id.SignUpButton);
			mButtonSignIn = FindViewById<Button> (Resource.Id.SignInButton);
			mProgressBar = FindViewById<ProgressBar> (Resource.Id.progressBar1);
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

			//starts up our messaging service
			msgText = FindViewById<TextView> (Resource.Id.msgText);

			if (IsPlayServicesAvailable ())
			{
				var intent = new Intent (this, typeof (RegistrationIntentService));
				StartService (intent);
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
			userEmailSignIn = e.Email;
			userPasswordSignIn = e.Password;
			Thread thread = new Thread(ActLikeARequest);
			thread.Start ();
			//string userPassword = e.Password;

			//Post to server code
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
		void signUpDialog_mOnSignUpComplete(Object sender, OnSignUpEventArgs e)
		{
			// here we send request to server
			// just simulating here
			Thread thread = new Thread(ActLikeARequest);
			thread.Start ();
			//string userPassword = e.Password;

			userNameSignUp = e.UserName;
			userEmailSignUp = e.Email;
			userPasswordSignUp = e.Password;

			// Post to server code
		}

		// Part of thread simulation

		/// <summary>
		/// Acts like A request.
		/// </summary>
		private async void ActLikeARequest()
		{
            Credentials test = new Credentials("houzec8");

            var loggedIn = await test.doLogin("passsdflkj", serverURL + loginExt + "/");

            if (loggedIn)
            {
                System.Diagnostics.Debug.WriteLine("Successfully signed in, token is: " + test.token);
                var testProfile = await Getter<Profile>.GetObject(test.username, serverURL + profileExt + "/");
                testProfile.token = test.token;

                if(testProfile != default(Profile))
                {
                    testProfile.current_lat = 24;
                    testProfile.current_long = 31;
                    await Updater.UpdateObject(testProfile, serverURL + locationExt + "/");
                }
            }

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
    }
}


