
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

			//add code for buttons that are at the bottom

		
		}

	
	
	
		protected void SendNewMsg()
		{
			//open send msg dialog 
			FragmentTransaction transaction = FragmentManager.BeginTransaction();
			MessagingActivity sendMsgDialog = new MessagingActivity();
			MessagingActivity.Show(transaction, "Dialog Fragment");
			this.Dismiss();
		}

		protected void Close()
		{
			this.Dismiss();
		}

	}
}