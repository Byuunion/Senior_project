
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

using MeetMeet_Native_Portable;
using Android.Content.PM;


/// <summary>
/// Invite request activity. This activity starts when receiving and invite request
/// in the user profile section. 
/// </summary>
namespace MeetMeet_Native_Portable.Droid
{
	[Activity (Label = "InviteRequestActivity")]			
	public class InviteRequestActivity : Activity
	{
		// Declaration of buttons and text view in invite_request layout
		private Button mBtnCheckProfileInvite;
		private Button mBtnAcptMeetInvite;
		private Button mBtnDclnMeetInvite;
		private TextView mUsernameInviteTextView;

		// Necessary server variables 
		public static Credentials credentials;
		public static string serverURL = "http://52.91.212.179:8800/";

		// Stores username of invitee
		public string userNameFrom;

		/// <summary>
		/// Starts event when receiving an invite request
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		 protected override void OnCreate (Bundle bundle)
		{

			base.OnCreate (bundle);

			// Set our view from the home_page resource
			SetContentView (Resource.Layout.invite_request);

			//Retrieves username passed from GcmListenerService
			userNameFrom = Intent.GetStringExtra ("username_from") ?? "Data not available";

			// Setting Button and Textview References from invite_request layout
			mBtnCheckProfileInvite = FindViewById<Button> (Resource.Id.BtnInviteCheckProfile);
			mBtnAcptMeetInvite = FindViewById<Button> (Resource.Id.BtnAcptMeetInvitation);
			mBtnDclnMeetInvite = FindViewById<Button> (Resource.Id.BtnDeclineMeetInvitation);
			mUsernameInviteTextView = FindViewById<TextView> (Resource.Id.UsernameInviteTextView);

			// Set username of invitee to textview in invite_request layout
			mUsernameInviteTextView.Text = userNameFrom;


			//Button Function

			// When user clicks Check Profile button in invite_request layout, this 
			// starts view profile activity. Also passes username of invitee to the activity.
			mBtnCheckProfileInvite.Click += delegate {
				Intent intent = new Intent(this, typeof(ViewProfile));
				intent.PutExtra ("username_from", userNameFrom);
				StartActivity(intent);
			};

			mBtnAcptMeetInvite.Click += MBtnAcptMeetInvite_Click;

			mBtnDclnMeetInvite.Click += MBtnDclnMeetInvite_Click;

		}

		/// <summary>
		/// When Decline invite button is clicked, it will send via MessengerSender a "false" response
		/// If it passes to the server. A toast will appear verifying "Response sent" and exit.
		/// If it is unable to pass it to server a "response failed" toast will appear to user.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		 async void MBtnDclnMeetInvite_Click (object sender, EventArgs e)
		 {
			if (await MessageSender.RespondGroupInvite (userNameFrom, MainActivity.credentials, serverURL + MainActivity.group_invite, "false")) {
				Toast.MakeText (this, "Response Sent!", ToastLength.Short).Show ();
				base.OnBackPressed ();
			} else {
				Toast.MakeText (this, "Response Failed!", ToastLength.Short).Show ();
			}
		 }

		/// <summary>
		/// When Accept button invite is clicked it will send via MessengerSender a "true" response
		/// If it passes to the server. A toast will appear verifying "Response sent" and exit.
		/// If it is unable to pass it to server a "response failed" toast will appear to user.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		 async void MBtnAcptMeetInvite_Click (object sender, EventArgs e)
		 {
			if (await MessageSender.RespondGroupInvite (userNameFrom, MainActivity.credentials, serverURL + MainActivity.group_invite, "true")) {

				await MessageSender.SendSingleMessage (MainActivity.credentials.username + " accepts your invite!", userNameFrom, MainActivity.credentials, serverURL + MainActivity.single_message);  

				Toast.MakeText (this, "Response Sent!", ToastLength.Short).Show ();
				base.OnBackPressed ();
			} else {
				Toast.MakeText (this, "Response Failed!", ToastLength.Short).Show ();
			}
		 }
	}
}

