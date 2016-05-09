using System;

using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MeetMeet_Native_Portable.Droid
{
	/// <summary>
	/// The nearby profile fragment handles the display of
	/// the of the profile bios of the selected user from
	/// the list fragment.
	/// </summary>
    internal class NearbyProfileFragment : Fragment
    {
		// String array of nearby profile bios
		private static string[] nearbyBios;

		/// <summary>
		/// Creates a new fragment instance to display the bios
		/// of the selected username from the nearby user list.
		/// </summary>
		/// <param name="userId">Index of the current selected username</param>
		/// <param name="bios">Array of all the nearby users' bios</param>
        public static NearbyProfileFragment NewInstance(int userId, string[] bios)
        {
			nearbyBios = bios;
			var nearbyProfileFrag = new NearbyProfileFragment {Arguments = new Bundle()};
			nearbyProfileFrag.Arguments.PutInt("selected_user_id", userId);
            return nearbyProfileFrag;
        }

		/// <summary>
		/// Gets the current index of the selected username from the nearby user list.
		/// </summary>
        public int ShownUserId
        {
			get { return Arguments.GetInt("selected_user_id", 0); }
        }

		/// <summary>
		/// Creates a new view for the profile fragment whenever a new username is selected 
		/// and displays the corresponding profile bios.
		/// <param name="inflater">Android layout inflater.</param>
		/// <param name="container">.ndroid view container</param>
		/// <param name="savedInstanceState">Bundle.</param>
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