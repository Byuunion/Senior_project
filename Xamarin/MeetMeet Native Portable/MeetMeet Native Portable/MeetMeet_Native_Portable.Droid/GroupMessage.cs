
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

using Android.Support.V4.App;
using Android.Locations;
using System.Threading.Tasks;




namespace MeetMeet_Native_Portable.Droid
{
	[Activity (Label = "Group Message")]			
	public class GroupMessage : Activity
	{
		private EditText mMessage;
		private Button mSendMessage;
		public static string serverURL = "http://52.91.212.179:8800/";




		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view 
			SetContentView (Resource.Layout.group_message);

			mMessage = FindViewById<EditText> (Resource.Id.sendMessageTxt);
			mSendMessage = FindViewById<Button> (Resource.Id.btnSendMsg);


			mSendMessage.Click += MSendMessage_Click;
		}

		async void MSendMessage_Click (object sender, EventArgs e)
		{
			if (await MessageSender.SendGroupMessage (mMessage.Text, MainActivity.credentials, serverURL + MainActivity.group_message)) {
				Message m = new Message ();
				m.Date = System.DateTime.Now.ToString ();
				m.UserName = "group";
				m.MsgText = mMessage.Text;
				m.incoming = false;
				MessageRepository.SaveMessage (m);
				mMessage.Text = "";
				Toast.MakeText (this, "Message Sent!", ToastLength.Short).Show ();
			} else {
				Toast.MakeText (this, "Message Failed!", ToastLength.Short).Show ();
			}
		}


	}
}
