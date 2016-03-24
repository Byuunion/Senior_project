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
		public string userEmail;
		public string userPassword;
        public static string serverURL = "http://52.91.212.179:8801/{0}";

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
			userEmail = e.Username;
			userPassword = e.Password;
			Thread thread = new Thread(ActLikeARequest);
			thread.Start ();
			//string userPassword = e.Password;
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
		}

		// Part of thread simulation

		/// <summary>
		/// Acts like A request.
		/// </summary>
		private async void ActLikeARequest()
		{
            //Thread.Sleep (3000);
            /*
            string tag = "MeetMeet";
		
			//Profile test = new Profile(userEmail, 5, 6, userPassword);
			Profile test = new Profile("Hi", 5, 6, "Bye");
            await Poster.PostObject(test, "http://52.91.212.179:8800/user/");

            var test2 = await Getter<Profile>.GetObject(test.username, "http://52.91.212.179:8800/user/profile/{0}");
            if(test2 == default(Profile))
            {
                System.Diagnostics.Debug.WriteLine("Response received was null");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Response received first name: {0} last name: {1}", test2.gender, test2.bio);
            }*/

            Login test = new Login("GrilloPad");
            var loggedIn = await test.doLogin("hunter2", serverURL);

            if (loggedIn)
            {
                System.Diagnostics.Debug.WriteLine("Successfully logged in, token is: " + test.token);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Could not log in");
            }
           //await Deleter.DeleteProfile("TestPost", "http://52.91.212.179:8800/user/{0}");

            //RunOnUiThread (() => {mProgressBar.Visibility = ViewStates.Invisible; });
        }
    }
}


