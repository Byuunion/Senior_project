
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

// Class to change layout when SignUp button pressed

namespace MeetMeet_Native_Portable.Droid
{
	/// <summary>
	/// On sign up event arguments.
	/// Gets and Sets user input for firstname
	/// email and password.
	/// </summary>
	public class OnSignUpEventArgs : EventArgs {

		private string mUserName;
		private string mEmail;
		private string mPassword;

		/// <summary>
		/// Gets or sets the first name.
		/// </summary>
		/// <value>The first name.</value>
		public string UserName
		{
			get { return mUserName; }
			set { mUserName = value; }
		}

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		public string Email 
		{
			get { return mEmail; }
			set { mEmail = value; }
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

		/// <summary>
		/// Initializes a new instance of the <see cref="MeetMeet_Native_Portable.Droid.OnSignUpEventArgs"/> class.
		/// </summary>
		/// <param name="firstName">First name.</param>
		/// <param name="email">Email.</param>
		/// <param name="password">Password.</param>
		public OnSignUpEventArgs(String userName, String email, string password) : base()
		{
			UserName = userName;
			Email = email;
			Password = password;


		}
			
	}

	/// <summary>
	/// Provides functionality to Sign Up dialog fragment.
	/// </summary>
	[Activity (Label = "SignUp")]			
	class SignUp : DialogFragment
	{
		private EditText mTxtUserName;
		private EditText mTxtEmail;
		private EditText mTxtPassword;
		private Button mBtnSignUp;

		/// <summary>
		/// The m on sign up complete.
		/// </summary>
		public EventHandler<OnSignUpEventArgs> mOnSignUpComplete;

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

			var view = inflater.Inflate (Resource.Layout.dialog_sign_up, container, false);

			// Resource for SignUp text
			mTxtUserName=  view.FindViewById<EditText>(Resource.Id.txtUserName);
			mTxtEmail=  view.FindViewById<EditText>(Resource.Id.txtEmail);
			mTxtPassword=  view.FindViewById<EditText>(Resource.Id.txtPassword);
			mBtnSignUp = view.FindViewById<Button> (Resource.Id.btnDialogEmail);

			// Click event

			mBtnSignUp.Click += MBtnSignUp_Click;

			return view;
		}

		// Occurs when user clicks Sign up Button 

		/// <summary>
		/// Completes Sign up process and passes arguments. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void MBtnSignUp_Click (object sender, EventArgs e)
		{

			mOnSignUpComplete.Invoke (this, new OnSignUpEventArgs (mTxtUserName.Text, mTxtEmail.Text, mTxtPassword.Text));
			this.Dismiss ();
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