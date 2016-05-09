using Android.App;
using Android.OS;
using Android.Support.V4.App;

namespace MeetMeet_Native_Portable.Droid
{
	/// <summary>
	/// Nearby users activity. This fragment activity starts
	/// when a person is trying to find nearby users.
	/// Sets layout to nearby_users.
	/// </summary>
	[Activity(Label = "Nearby Users")]
	public class NearbyUsersActivity : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.nearby_users);
		}
	}
}