using Android.App;
using Android.OS;
using Android.Support.V4.App;

namespace MeetMeet_Native_Portable.Droid
{
	[Activity(Label = "Fragments Walkthrough")]
	public class ProfileMainActivity : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_main);
		}
	}
}