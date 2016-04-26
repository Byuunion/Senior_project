using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using MeetMeet_Native_Portable.Droid;
using Android.Media;

namespace ClientApp
{
    /// <summary>
    /// Listener for Google Cloud Messages
    /// </summary>
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {
        /// <summary>
        /// This method gets called when a message is received by the listener
        /// </summary>
        /// <param name="from">The user who send the message</param>
        /// <param name="data">The data of the message, includes the message text and message code</param>
        public override void OnMessageReceived(string from, Bundle data)
        {
            var message = data.GetString("message");
            Log.Debug("MyGcmListenerService", "From:    " + from);
            Log.Debug("MyGcmListenerService", "Message: " + message);
            
			int ms_code = 0;
			int.TryParse((data.GetString ("message_code")), out ms_code);

			string username = data.GetString ("username_from");
			System.Diagnostics.Debug.WriteLine (username);

			MeetMeet_Native_Portable.Droid.Message m = new MeetMeet_Native_Portable.Droid.Message ();


			m.MsgText = message;
			System.Diagnostics.Debug.WriteLine (m.MsgText);
			m.Date = System.DateTime.Now.ToString();
			m.incoming = true;
			
            if(ms_code == 1)
            {
                //This is for single messages
				m.UserName = username;
				MessageRepository.SaveMessage (m);
                SendNotification(message, username);
            }
            else if(ms_code == 2)
            {
                //This is for group invites
				Intent intent = new Intent(this, typeof(InviteRequestActivity));
				intent.PutExtra ("username_from", username);
				intent.SetFlags (ActivityFlags.NewTask);
				StartActivity(intent);
			}
            else if(ms_code == 3){
				//This is for group messages
				m.UserName = "group, sent by " + username;
				MessageRepository.SaveMessage (m);
                SendNotification(message, m.UserName);
            }

            //Try to add it to the inbox, if the inbox has been created.
            //Otherwise, it will get added automatically when the user opens the inbox
			ViewInbox inbox = (ViewInbox)MainActivity.references.Get ("Inbox");
            if(inbox != null)
            {
                inbox.newMessage (m);
            }
			

        }

        /// <summary>
        /// Send a notification to the user's notification bar
        /// </summary>
        /// <param name="message">The message text</param>
        /// <param name="username">The user who sent the message</param>
		void SendNotification(string message, string username)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
                .SetContentTitle("Message from: " + username)
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationBuilder.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification));
            notificationManager.Notify(0, notificationBuilder.Build());


        }
    }
}