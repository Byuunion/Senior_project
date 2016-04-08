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
	public class NearbyUsersFragment : ListFragment
	{
		private int selectedUserId;
		private bool isDualPane;
		private LocationManager locationManager;
		private String provider;
		private string[] nearbyBios;

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			var ProfileFrame = Activity.FindViewById<View>(Resource.Id.UserProfile);

			// If running on a tablet, then the layout in Resources/Layout-Large will be loaded. 
			// That layout uses fragments, and defines the detailsFrame. We use the visiblity of 
			// detailsFrame as this distinguisher between tablet and phone.
			isDualPane = ProfileFrame != null && ProfileFrame.Visibility == ViewStates.Visible;


			// Get the location manager
			locationManager = (LocationManager) Config.context.GetSystemService(Context.LocationService);
			// Define the criteria how to select the location provider -> use default
			Criteria criteria = new Criteria();
			criteria.Accuracy = Accuracy.Fine;
			provider = locationManager.GetBestProvider(criteria, true);
			Location location = locationManager.GetLastKnownLocation(provider);
			Geolocation currentLocation = new Geolocation (MainActivity.username, location.Latitude, location.Longitude);
			//Geolocation currentLocation = new Geolocation (MainActivity.username, 39.77689537, -75.11926562);

			Task<List<Profile>> task3 = Task<List<Profile>>.Factory.StartNew(() => 
				{ 
					return currentLocation.GetNearbyProfiles().Result;
				});
			
			List<Profile> nearbyUserslist = new List<Profile>();
			try{
				nearbyUserslist = task3.Result;
			}
			catch(Exception e) {
				string error = e.Message;
				System.Diagnostics.Debug.WriteLine ("\n\nServer offline\n\n" + error);
			}


			int numUsers = nearbyUserslist.Count;
			string[] nearbyUsers = new string[numUsers];
			nearbyBios = new string[numUsers];

			for (int i = 0; i < numUsers; i++)
			{
				nearbyUsers [i] = nearbyUserslist [i].username;
				nearbyBios [i] = nearbyUserslist [i].bio;
			}
		
			var adapter = new ArrayAdapter<String>(Activity, Android.Resource.Layout.SimpleListItemChecked, nearbyUsers);
			ListAdapter = adapter;

			if (savedInstanceState != null)
			{
				selectedUserId = savedInstanceState.GetInt("selected_user_id", 0);
			}

			if (isDualPane)
			{
				ListView.ChoiceMode = ChoiceMode.Single;
				ShowProfile(selectedUserId);
			}
		}

		public override void OnSaveInstanceState(Bundle outState)
		{  
			base.OnSaveInstanceState(outState);
			outState.PutInt("selected_user_id", selectedUserId);
		}

		public override void OnListItemClick(ListView l, View v, int position, long id)
		{
			ShowProfile(position);
		}

		private void ShowProfile(int userId)
		{
			selectedUserId = userId;
			if (isDualPane)
			{
				// We can display everything in-place with fragments.
				// Have the list highlight this item and show the data.
				ListView.SetItemChecked(userId, true);

				// Check what fragment is shown, replace if needed.
				var nearbyProfile = FragmentManager.FindFragmentById(Resource.Id.UserProfile) as NearbyProfileFragment;
				if (nearbyProfile == null || nearbyProfile.ShownUserId != userId)
				{
					// Make new fragment to show this selection.
					nearbyProfile = NearbyProfileFragment.NewInstance(userId, nearbyBios);

					// Execute a transaction, replacing any existing
					// fragment with this one inside the frame.
					var ft = FragmentManager.BeginTransaction();
					ft.Replace(Resource.Id.UserProfile, nearbyProfile);
					ft.SetTransition(FragmentTransaction.TransitFragmentFade);
					ft.Commit();
				}
			}
			else
			{
				// Otherwise we need to launch a new activity to display
				// the nearby profile fragment with user data.
				var intent = new Intent();

				intent.SetClass(Activity, typeof (NearbyProfileActivity));
				intent.PutExtra("selected_user_id", userId);
				StartActivity(intent);
			}
		}
	}
}