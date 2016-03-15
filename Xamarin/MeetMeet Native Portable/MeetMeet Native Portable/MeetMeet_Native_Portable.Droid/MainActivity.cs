using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
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
    [Activity(Label = "MeetMeet_Native_Portable.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

		// Main Menu Items
		private Button mButtonSignUp;
		private Button mButtonSignIn;
		private ProgressBar mProgressBar;

		/// <summary>
		/// Creates the event for main activity.
		/// Sets layout to main, references the main page button,
		/// and signIn/signOut button clicks.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Profile test = new Profile("test2", "no", "name", "nope");
            System.Diagnostics.Debug.WriteLine("hello");
            Poster.PostObject(test, "http://52.91.212.179:8800/user/");
            System.Diagnostics.Debug.WriteLine("Got here");
           
			// Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

			//References for Main Menu Items
			mButtonSignUp = FindViewById<Button> (Resource.Id.SignUpButton);
			mButtonSignIn = FindViewById<Button> (Resource.Id.SignInButton);
			mProgressBar = FindViewById<ProgressBar> (Resource.Id.progressBar1);

			//Click Events

			//Sign Up Click
			mButtonSignUp.Click += mButtonSignUp_Click;

			//Sign in Click opens 
			mButtonSignIn.Click += MButtonSignIn_Click;

	
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
		private void ActLikeARequest()
		{
			Thread.Sleep (3000);

			RunOnUiThread (() => {mProgressBar.Visibility = ViewStates.Invisible; });
		}
    }
}


