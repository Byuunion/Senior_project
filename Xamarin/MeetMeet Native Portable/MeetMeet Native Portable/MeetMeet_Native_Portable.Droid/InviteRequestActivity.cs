
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

			userNameFrom = Intent.GetStringExtra ("usernameFrom") ?? "Data not available";

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

			// mBtnCheckProfileInvite.Click += Pull Profile or perhaps have slide fragment to be 

			mBtnAcptMeetInvite.Click += delegate {
				//InviteAccept ();

				MessageSender.RespondGroupInvite(userNameFrom,credentials, serverURL,"Lets Meet!");
			};
	

			mBtnDclnMeetInvite.Click += delegate {

				base.OnBackPressed ();
			};

			//return view;
		}
	}
}

