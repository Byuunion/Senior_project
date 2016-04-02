
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

namespace MeetMeet_Native_Portable.Droid
{
	[Activity (Label = "InviteRequestActivity")]			
	public class InviteRequestActivity : DialogFragment
	{
		private Button mBtnCheckProfileInvite;
		private Button mBtnAcptMeetInvite;
		private Button mBtnDclnMeetInvite;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
			base.OnCreateView (inflater, container, savedInstanceState);

			var view = inflater.Inflate (Resource.Layout.invite_request, container, false);

			// Button Reference

			mBtnCheckProfileInvite = view.FindViewById<Button> (Resource.Id.BtnInviteCheckProfile);
			mBtnAcptMeetInvite = view.FindViewById<Button> (Resource.Id.BtnAcptMeetInvitation);
			mBtnDclnMeetInvite = view.FindViewById<Button> (Resource.Id.BtnDeclineMeetInvitation);

			//Button Function

			// mBtnCheckProfileInvite.Click += 

			//mBtnAcptMeetInvite.Click += MessagingCenter.Send<HomeActivity> (this, "Accept");

			//mBtnDclnMeetInvite.Click += MessagingCenter.Send<HomeActivity> (this, "Decline");

			return view;
			}
}
}

