
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

namespace MeetMeet_Native_Portable.Droid
{

	public class OnSignInEventArgs: EventArgs 
	{
		private string mEmail;
		private string mPassword;

		public string Email 
		{
			get { return mEmail; }
			set { mEmail = value; }
		}

		public string Password 
		{
			get { return mPassword; }
			set { mPassword = value; }
		}

		public OnSignInEventArgs(String email, String password) : base()
		{
			Email = email;
			Password = password;

		}

	}
	[Activity (Label = "SignIn")]			
	public class SignIn : DialogFragment
	{
		private EditText mTxtEmailSignIn;
		private EditText mTxtPasswordSignIn;
		private Button mBtnDialogSignIn;

		public EventHandler <OnSignInEventArgs> mOnSignInComplete;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);

			var view = inflater.Inflate (Resource.Layout.dialog_sign_in, container, false);

			// Resource for SignIn
			mTxtEmailSignIn = view.FindViewById<EditText>(Resource.Id.txtEmailSignIn);
			mTxtPasswordSignIn = view.FindViewById<EditText>(Resource.Id.txtPasswordSignIn);
			mBtnDialogSignIn = view.FindViewById<Button> (Resource.Id.btnDialogSignIn);

			// Sign In Click Event

			mBtnDialogSignIn.Click += MBtnDialogSignIn_Click;


			return view;
		}

		// This method will need bring to profile or new user email where they can search
		void MBtnDialogSignIn_Click (object sender, EventArgs e)
		{
			mOnSignInComplete.Invoke (this, new OnSignInEventArgs (mTxtEmailSignIn.Text, mTxtPasswordSignIn.Text));
			this.Dismiss ();
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			Dialog.Window.RequestFeature (WindowFeatures.NoTitle);
			base.OnActivityCreated (savedInstanceState);

		}
	}
}

