using Android.App;
using Android.Content;
using Android.Gms.Gcm.Iid;

namespace ClientApp
{
    /// <summary>
    /// GCM Listener
    /// </summary>
	[Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
	class MyInstanceIDListenerService : InstanceIDListenerService
	{
        /// <summary>
        /// Called when the Google system determines that the tokens need to be refreshed
        /// </summary>
		public override void OnTokenRefresh()
		{
			var intent = new Intent (this, typeof (RegistrationIntentService));
			StartService (intent);
		}
	}
}