using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;

namespace ClientApp
{
	[Service(Exported = false)]
	class RegistrationIntentService : IntentService
	{
		static object locker = new object();

		public RegistrationIntentService() : base("RegistrationIntentService") { }

		protected override void OnHandleIntent (Intent intent)
		{
			try
			{
				Log.Info ("RegistrationIntentService", "Calling InstanceID.GetToken");
				lock (locker)
				{
					var instanceID = InstanceID.GetInstance (this);
					var token = instanceID.GetToken (
						"47160214403", GoogleCloudMessaging.InstanceIdScope, null);
						//senderID generated from google dev. console
					Log.Info ("RegistrationIntentService", "GCM Registration Token: " + token);
					SendRegistrationToAppServer (token);
					Subscribe (token);
				}
			}
			catch (Exception e)
			{
				Log.Debug("RegistrationIntentService", "Failed to get a registration token");
				return;
			}
		}

		void SendRegistrationToAppServer (string token)
		{
			// Add custom implementation here as needed.
		}

		void Subscribe (string token)
		{
			var pubSub = GcmPubSub.GetInstance(this);
			pubSub.Subscribe(token, "/topics/global", null);
		}
	}
}