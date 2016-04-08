using Android.App;
using Android.OS;
using Android.Support.V4.App;

namespace MeetMeet_Native_Portable.Droid
{
[Activity(Label = "Nearby User Profile")]
public class NearbyProfileActivity : FragmentActivity
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        var index = Intent.Extras.GetInt("current_play_id", 0);

		var nearbyProfile = NearbyProfileFragment.NewInstance(index, null); // Details
        var fragmentTransaction = SupportFragmentManager.BeginTransaction();
        fragmentTransaction.Add(Android.Resource.Id.Content, nearbyProfile);
        fragmentTransaction.Commit();
    }
}
}