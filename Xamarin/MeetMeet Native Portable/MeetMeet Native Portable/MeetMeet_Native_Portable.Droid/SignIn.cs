
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
	/// <summary>
	/// Handles Sign In event arguments.
	/// </summary>
	public class OnSignInEventArgs: EventArgs 
	{
		private string mUserName;
		private string mPassword;
		//private string mEmail;

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		public string Username 
		{
			get { return mUserName; }
			set { mUserName = value; }
		}

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public string Password 
		{
			get { return mPassword; }
			set { mPassword = value; }
		}

		//public string Email 
		//{
			//get { return mEmail; }
			//set { mEmail = value; }
		//}

		/// <summary>
		/// Initializes a new instance of the <see cref="MeetMeet_Native_Portable.Droid.OnSignInEventArgs"/> class.
		/// </summary>
		/// <param name="email">Email.</param>
		/// <param name="password">Password.</param>
		public OnSignInEventArgs(String userName, String password) : base()
		{
			Username = userName;
			Password = password;
			//Email = email;
		}

	}

	/// <summary>
	/// Provides functionality for Sign In dialog fragment
	/// </summary>
	[Activity (Label = "SignIn")]			
	public class SignIn : DialogFragment
	{
		private EditText mTxtUserNameSignIn;
		private EditText mTxtPasswordSignIn;
		//private EditText mTxtEmailSignIn;
		private Button mBtnDialogSignIn;

		/// <summary>
		/// The m on sign in complete.
		/// </summary>
		public EventHandler <OnSignInEventArgs> mOnSignInComplete;

		/// <param name="inflater">The LayoutInflater object that can be used to inflate
		///  any views in the fragment,</param>
		/// <param name="container">If non-null, this is the parent view that the fragment's
		///  UI should be attached to. The fragment should not add the view itself,
		///  but this can be used to generate the LayoutParams of the view.</param>
		/// <param name="savedInstanceState">If non-null, this fragment is being re-constructed
		///  from a previous saved state as given here.</param>
		/// <summary>
		/// Called to have the fragment instantiate its user interface view.
		/// </summary>
		/// <returns>To be added.</returns>
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);

			var view = inflater.Inflate (Resource.Layout.dialog_sign_in, container, false);

			// Resource for SignIn
			mTxtUserNameSignIn = view.FindViewById<EditText>(Resource.Id.txtUserNameSignIn);
			mTxtPasswordSignIn = view.FindViewById<EditText>(Resource.Id.txtPasswordSignIn);
			//mTxtEmailSignIn = view.FindViewById<EditText>(Resource.Id.txtEmailSignIn);
			mBtnDialogSignIn = view.FindViewById<Button> (Resource.Id.btnDialogSignIn);

			// Sign In Click Event

			mBtnDialogSignIn.Click += MBtnDialogSignIn_Click;


			return view;
		}

		/// <summary>
		/// Completes Sign in process and passes arguments. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void MBtnDialogSignIn_Click (object sender, EventArgs e)
		{
			if (mTxtUserNameSignIn.Text != "" && mTxtPasswordSignIn.Text != "") 
			{
				mOnSignInComplete.Invoke (this, new OnSignInEventArgs (mTxtUserNameSignIn.Text, mTxtPasswordSignIn.Text));
				this.Dismiss ();
			}
		}

		/// <param name="savedInstanceState">If the fragment is being re-created from
		///  a previous saved state, this is the state.</param>
		/// <summary>
		/// Called when the fragment's activity has been created and this
		///  fragment's view hierarchy instantiated.
		/// </summary>
		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			Dialog.Window.RequestFeature (WindowFeatures.NoTitle);
			base.OnActivityCreated (savedInstanceState);

		}
	}
}


