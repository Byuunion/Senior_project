using System;

using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;

using MeetMeet_Native_Portable.Droid;

namespace ClientApp
{
    /// <summary>
    /// Register this device with Google Cloud Messaging
    /// </summary>
	[Service(Exported = false)]
	class RegistrationIntentService : IntentService
	{   
		static object locker = new object();

		public RegistrationIntentService() : base("RegistrationIntentService") { }

        /// <summary>
        /// Register this device with GCM to receive a Google Cloud Messaging id
        /// </summary>
        /// <param name="intent"></param>
		protected override void OnHandleIntent (Intent intent)
		{
			try
			{
				Log.Info ("RegistrationIntentService", "Calling InstanceID.GetToken");
				lock (locker)
				{
					var instanceID = InstanceID.GetInstance (this);

                    //The token is gotten twice due to a bug that was encountered where Google would assign
                    //a device an old, invalid token and fail to mark the token as valid again.
                    //This was fixed by getting the old token, forcibly deleting it and requesting a new token
					var token = instanceID.GetToken (
						"47160214403", GoogleCloudMessaging.InstanceIdScope, null);
						//senderID generated from google dev. console
					Log.Info ("RegistrationIntentService", "GCM Registration Token: " + token);

                    instanceID.DeleteToken(token, GoogleCloudMessaging.InstanceIdScope);
                    instanceID.DeleteInstanceID();

                    instanceID = InstanceID.GetInstance(this);
                    token = instanceID.GetToken(
                        "47160214403", GoogleCloudMessaging.InstanceIdScope, null);


                    //Make the token accessible by our code
                    MainActivity.gcm_token = token;
					Subscribe (token);
				}
			}
			catch (Exception e)
			{
				Log.Debug("RegistrationIntentService", "Failed to get a registration token");
				return;
			}
		}

        /// <summary>
        /// Have this device subscribe to the general topic
        /// </summary>
        /// <param name="token">The Google id of this device</param>
		void Subscribe (string token)
		{
			var pubSub = GcmPubSub.GetInstance(this);
			pubSub.Subscribe(token, "/topics/global", null);
		}
    }
}