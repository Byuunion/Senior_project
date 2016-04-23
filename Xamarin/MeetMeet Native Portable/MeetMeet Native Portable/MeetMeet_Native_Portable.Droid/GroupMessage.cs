using System;

using Android.App;
using Android.OS;
using Android.Widget;


namespace MeetMeet_Native_Portable.Droid
{
    /// <summary>
    /// Group messaging activity
    /// </summary>
	[Activity (Label = "Group Message")]			
	public class GroupMessage : Activity
	{
		private EditText mMessage;
		private Button mSendMessage;

        /// <summary>
        /// Code to execute when the activity is created
        /// </summary>
        /// <param name="bundle">Data passed to the activity</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view 
			SetContentView (Resource.Layout.group_message);

			mMessage = FindViewById<EditText> (Resource.Id.sendMessageTxt);
			mSendMessage = FindViewById<Button> (Resource.Id.btnSendMsg);

			mSendMessage.Click += MSendMessage_Click;
		}

        /// <summary>
        /// Button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		async void MSendMessage_Click (object sender, EventArgs e)
		{
			if (await MessageSender.SendGroupMessage (mMessage.Text, MainActivity.credentials, MainActivity.serverURL + MainActivity.group_message))
            {
				Message m = new Message ();

				m.Date = System.DateTime.Now.ToString ();
				m.UserName = "group";
				m.MsgText = mMessage.Text;
				m.incoming = false;

				MessageRepository.SaveMessage (m);

				mMessage.Text = "";

				Toast.MakeText (this, "Message Sent!", ToastLength.Short).Show ();
			}
            else
            {
				Toast.MakeText (this, "Message Failed!", ToastLength.Short).Show ();
			}
		}


	}
}
