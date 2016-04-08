using System;

using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MeetMeet_Native_Portable.Droid
{
    internal class NearbyProfileFragment : Fragment
    {
		private static string[] nearbyBios;
		
        public static NearbyProfileFragment NewInstance(int userId, string[] bios)
        {
			nearbyBios = bios;
			var nearbyProfileFrag = new NearbyProfileFragment {Arguments = new Bundle()};
			nearbyProfileFrag.Arguments.PutInt("selected_user_id", userId);
            return nearbyProfileFrag;
        }

        public int ShownUserId
        {
			get { return Arguments.GetInt("selected_user_id", 0); }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            var scroller = new ScrollView(Activity);
            
            var text = new TextView(Activity);
            var padding = Convert.ToInt32(TypedValue.ApplyDimension(ComplexUnitType.Dip, 4, Activity.Resources.DisplayMetrics));
            text.SetPadding(padding, padding, padding, padding);
            text.TextSize = 24;

			try {
				text.Text = nearbyBios[ShownUserId];
			}
			catch(IndexOutOfRangeException e) {
				string error = "\n\nServer offline\n\n" + e.Message;
				System.Diagnostics.Debug.WriteLine (error);
				nearbyBios = new string[1];
				nearbyBios [0] = error;
			}
			

            scroller.AddView(text);

            return scroller;
        }
    }
}