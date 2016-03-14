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
    [Activity(Label = "MeetMeet_Native_Portable.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
		private Button mButtonSignUp;
		private ProgressBar mProgressBar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Profile test = new Profile("test2", "no", "name", "nope");
            System.Diagnostics.Debug.WriteLine("hello");
            Poster.PostObject(test, "http://52.91.212.179:8800/user/");
            System.Diagnostics.Debug.WriteLine("Got here");
           
			// Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
			mButtonSignUp = FindViewById<Button> (Resource.Id.SignUpButton);
			mProgressBar = FindViewById<ProgressBar> (Resource.Id.progressBar1);
			mButtonSignUp.Click += mButtonSignUp_Click;

			// Option: If you wanted click to open new Layout

			//Button buttonsignup = FindViewById<Button> (Resource.Id.SignUpButton);
			//buttonsignup.Click += delegate {
			//	SetContentView (Resource.Layout.Signup);
			//	StartActivity(typeof(SignUp2));;
			//};

        }

		void mButtonSignUp_Click (object sender, EventArgs e)
		{

			//Pull up dialog
			FragmentTransaction transaction = FragmentManager.BeginTransaction();
			SignUp signUpDialog = new SignUp();
			signUpDialog.Show(transaction, "Dialog Fragment");

			//Subscribing to Signup Event

			signUpDialog.mOnSignUpComplete += signUpDialog_mOnSignUpComplete;

		}


		void signUpDialog_mOnSignUpComplete(Object sender, OnSignUpEventArgs e)
		{
			// here we send request to server
			// just simulating here
			Thread thread = new Thread(ActLikeARequest);
			thread.Start ();
			//string userPassword = e.Password;
		}

		// Part of thread simulation
		private void ActLikeARequest()
		{
			Thread.Sleep (3000);

			RunOnUiThread (() => {mProgressBar.Visibility = ViewStates.Invisible; });
		}
    }
}


