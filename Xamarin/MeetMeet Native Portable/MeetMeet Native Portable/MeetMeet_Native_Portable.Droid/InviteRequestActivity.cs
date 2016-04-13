
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
//using Android.App.Activity;

namespace MeetMeet_Native_Portable.Droid
{
	[Activity (Label = "InviteRequestActivity")]			
	//public class InviteRequestActivity : DialogFragment
	public class InviteRequestActivity : Activity
	{
		private Button mBtnCheckProfileInvite;
		private Button mBtnAcptMeetInvite;
		private Button mBtnDclnMeetInvite;
		private TextView mUsernameInviteTextView;

		public static Credentials credentials;
		public static string serverURL = "http://52.91.212.179:8800/";

		public string userNameFrom;

		//public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		//{

		//	base.OnCreateView (inflater, container, savedInstanceState);

		//	var view = inflater.Inflate (Resource.Layout.invite_request, container, false);

		 protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the home_page resource
			SetContentView (Resource.Layout.invite_request);

			userNameFrom = Intent.GetStringExtra ("username_from") ?? "Data not available";

			// Button Reference

			//mBtnCheckProfileInvite = view.FindViewById<Button> (Resource.Id.BtnInviteCheckProfile);
			//mBtnAcptMeetInvite = view.FindViewById<Button> (Resource.Id.BtnAcptMeetInvitation);
			//mBtnDclnMeetInvite = view.FindViewById<Button> (Resource.Id.BtnDeclineMeetInvitation);

			mBtnCheckProfileInvite = FindViewById<Button> (Resource.Id.BtnInviteCheckProfile);
			mBtnAcptMeetInvite = FindViewById<Button> (Resource.Id.BtnAcptMeetInvitation);
			mBtnDclnMeetInvite = FindViewById<Button> (Resource.Id.BtnDeclineMeetInvitation);
			mUsernameInviteTextView = FindViewById<TextView> (Resource.Id.UsernameInviteTextView);

			// Set username text
			mUsernameInviteTextView.Text = userNameFrom;


			//Button Function

			mBtnCheckProfileInvite.Click += delegate {
				Intent intent = new Intent(this, typeof(ViewProfile));
				intent.PutExtra ("username_from", userNameFrom);
				StartActivity(intent);
			};

			mBtnAcptMeetInvite.Click += MBtnAcptMeetInvite_Click;

				//delegate {
				//InviteAccept ();

				//MessageSender.RespondGroupInvite(userNameFrom,credentials, serverURL,"true");

				//base.OnBackPressed();
			//};
	

			mBtnDclnMeetInvite.Click += MBtnDclnMeetInvite_Click;

			//return view;
		}

		 async void MBtnDclnMeetInvite_Click (object sender, EventArgs e)
		 {
			if (await MessageSender.RespondGroupInvite (userNameFrom, MainActivity.credentials, serverURL + MainActivity.group_invite, "false")) {
				Toast.MakeText (this, "Response Sent!", ToastLength.Short).Show ();
				base.OnBackPressed ();
			} else {
				Toast.MakeText (this, "Response Failed!", ToastLength.Short).Show ();
			}
		 }

		 async void MBtnAcptMeetInvite_Click (object sender, EventArgs e)
		 {
			if (await MessageSender.RespondGroupInvite (userNameFrom, MainActivity.credentials, serverURL + MainActivity.group_invite, "true")) {
				Toast.MakeText (this, "Response Sent!", ToastLength.Short).Show ();
				base.OnBackPressed ();
			} else {
				Toast.MakeText (this, "Response Failed!", ToastLength.Short).Show ();
			}


		 }
	}
}

