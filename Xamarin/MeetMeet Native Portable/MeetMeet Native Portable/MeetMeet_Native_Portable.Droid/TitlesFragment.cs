using System;

using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Android.Locations;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MeetMeet_Native_Portable.Droid
{
	public class TitlesFragment : ListFragment
	{
		private int _currentPlayId;
		private bool _isDualPane;
		private LocationManager locationManager;
		private String provider;

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			var detailsFrame = Activity.FindViewById<View>(Resource.Id.details);

			// If running on a tablet, then the layout in Resources/Layout-Large will be loaded. 
			// That layout uses fragments, and defines the detailsFrame. We use the visiblity of 
			// detailsFrame as this distinguisher between tablet and phone.
			_isDualPane = detailsFrame != null && detailsFrame.Visibility == ViewStates.Visible;


			// Get the location manager
			locationManager = (LocationManager) Config.context.GetSystemService(Context.LocationService);
			// Define the criteria how to select the location provider -> use default
			Criteria criteria = new Criteria();
			provider = locationManager.GetBestProvider(criteria, false);
			Location location = locationManager.GetLastKnownLocation(provider);
			Geolocation currentLocation = new Geolocation ("kshea12", location.Latitude, location.Longitude);
			//Geolocation currentLocation = new Geolocation ("kshea12", 39.75259742, -75.21786803);

			Task<List<Geolocation>> task3 = Task<List<Geolocation>>.Factory.StartNew(() => 
				{ 
					return currentLocation.GetNearbyUsers().Result;
				});
			List<Geolocation> nearbyUserslist = task3.Result;
			int numUsers = nearbyUserslist.Count;
			string[] nearbyUsers = new string[numUsers];

			for (int i = 0; i < numUsers; i++) 
				nearbyUsers [i] = nearbyUserslist [i].username;


			var adapter = new ArrayAdapter<String>(Activity, Android.Resource.Layout.SimpleListItemChecked, nearbyUsers);
			ListAdapter = adapter;

			if (savedInstanceState != null)
			{
				_currentPlayId = savedInstanceState.GetInt("current_play_id", 0);
			}

			if (_isDualPane)
			{
				ListView.ChoiceMode = ChoiceMode.Single;
				ShowDetails(_currentPlayId);
			}
			currentLocation.UpdateGeolocation ();
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutInt("current_play_id", _currentPlayId);
		}

		public override void OnListItemClick(ListView l, View v, int position, long id)
		{
			ShowDetails(position);
		}

		private void ShowDetails(int playId)
		{
			_currentPlayId = playId;
			if (_isDualPane)
			{
				// We can display everything in-place with fragments.
				// Have the list highlight this item and show the data.
				ListView.SetItemChecked(playId, true);

				// Check what fragment is shown, replace if needed.
				var details = FragmentManager.FindFragmentById(Resource.Id.details) as DetailsFragment;
				if (details == null || details.ShownPlayId != playId)
				{
					// Make new fragment to show this selection.
					details = DetailsFragment.NewInstance(playId);

					// Execute a transaction, replacing any existing
					// fragment with this one inside the frame.
					var ft = FragmentManager.BeginTransaction();
					ft.Replace(Resource.Id.details, details);
					ft.SetTransition(FragmentTransaction.TransitFragmentFade);
					ft.Commit();
				}
			}
			else
			{
				// Otherwise we need to launch a new activity to display
				// the dialog fragment with selected text.
				var intent = new Intent();

				intent.SetClass(Activity, typeof (DetailsActivity));
				intent.PutExtra("current_play_id", playId);
				StartActivity(intent);
			}
		}
	}
}