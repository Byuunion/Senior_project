using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using MeetMeet_Native_Portable.Droid;

namespace ClientApp
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {
        public override void OnMessageReceived(string from, Bundle data)
        {
            var message = data.GetString("message");
            Log.Debug("MyGcmListenerService", "From:    " + from);
            Log.Debug("MyGcmListenerService", "Message: " + message);
            SendNotification(message);
			int ms_code = 0;
			int.TryParse((data.GetString ("message_code")), out ms_code);

			string username = data.GetString ("username_from");

            if(ms_code == 1)
            {
                /*
                *Put single user messages call here
                */
            }
            else if(ms_code == 2)
            {
                /*
                *Put group user message call here
                */
            }
            else if(ms_code == 3){

				//this may work
				Intent intent = new Intent(this, typeof(InviteRequestActivity));
				intent.PutExtra ("username_from", username);
				StartActivity(intent);
            }
			//string username = data.GetString ("username_from");
        }

        void SendNotification(string message)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
                .SetContentTitle("GCM Message")
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}