using Android.App;
using Android.OS;
using Android.Support.V4.App;

namespace MeetMeet_Native_Portable.Droid
{
[Activity(Label = "Details Activity")]
public class DetailsActivity : FragmentActivity
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        var index = Intent.Extras.GetInt("current_play_id", 0);

        var details = DetailsFragment.NewInstance(index, null); // Details
        var fragmentTransaction = SupportFragmentManager.BeginTransaction();
        fragmentTransaction.Add(Android.Resource.Id.Content, details);
        fragmentTransaction.Commit();
    }
}
}