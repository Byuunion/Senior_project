
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
	[Activity (Label = "MessagingActivity")]			
	public class MessagingActivity : DialogFragment
	{

		private Button mBtnSendMsg;
		private TextView mUsernameTextView;
		private TextView mMsgTextTextView;

		public static Credentials credentials;
		public static string serverURL = "http://52.91.212.179:8800/";

		public string userNameReciever;

		public override View OnCreateView(LayoutInflater inflator, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflator, container, savedInstanceState);
			var view = inflator.Inflate (Resource.Layout.dialog_send_msg, container, false);
			//SetContentView (Resource.Layout.dialog_send_msg);

			mUsernameTextView = view.FindViewById<EditText> (Resource.Id.editsenduser);
			mMsgTextTextView =	view.FindViewById<EditText> (Resource.Id.edittextmsg);
			mBtnSendMsg = view.FindViewById<Button> (Resource.Id.btnSendMsg); 
			// Create your application here

			mBtnSendMsg.Click += mBtnSendMsg_Click;
				

			return view;
		}

		void mBtnSendMsg_Click(object sender, EventArgs e)
		{
			MessageSender.SendSingleMessage( mMsgTextTextView.Text, mUsernameTextView.Text, MainActivity.credentials, "http://52.91.212.179:8800/user/message");
		}
	}
}

