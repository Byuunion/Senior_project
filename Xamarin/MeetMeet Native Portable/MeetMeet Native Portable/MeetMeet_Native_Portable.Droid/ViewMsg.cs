
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
	[Activity (Label = "ViewMsg")]			
	public class ViewMsg : DialogFragment
	{
		private Button mBtnSendNewMsg;
		private Button mBtnClose;
		private TextView mUsernameTextView;
		private TextView mMsgTextTextView;
	
	
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);

			var view = inflater.Inflate (Resource.Layout.dialog_view_msg, container, false);
	
			//this needs to display the username the message was from 
			//and the message when the user clicks on a message in the view inbox
			// ***** you will need to pass that info using json during the click event or get it from the message object? *****

			// References for Home Menu Items
			mBtnSendNewMsg = view.FindViewById<Button> (Resource.Id.sendMsgButton);
			mBtnClose = view.FindViewById<Button> (Resource.Id.closeMsgButton);
			mUsernameTextView = view.FindViewById<Button> (Resource.Id.userNameTextView);
			mMsgTextTextView = view.FindViewById<Button> (Resource.Id.textMsgView);

			// Set username text
			mUsernameTextView.Text = "username";

			// Set message text
			mMsgTextTextView.Text = "message text";

			//*** Click Events ***
			// Send New Message Click
			mBtnSendNewMsg.Click += MBtnSendNewMsg_Click;

			// Close Message Click
			mBtnClose.Click += MBtnClose_Click;


			/***Chris added this line so the project would compile***/
			return view;
			/********************************************************/
		}
	
		private void MBtnSendNewMsg_Click (object sender, EventArgs e)
		{
			//open send msg dialog 
			FragmentTransaction transaction = FragmentManager.BeginTransaction();
			MessagingActivity sendMsgDialog = new MessagingActivity();
			sendMsgDialog.Show(transaction, "Dialog Fragment");
			this.Dismiss();
		}

		private void MBtnClose_Click (object sender, EventArgs e)
		{
			this.Dismiss();
		}
	}
}