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
	/// <summary>
	/// The nearby users list fragment handles getting the user's current location 
	/// from their device and displays a list of nearby usernames
	/// </summary>
	public class NearbyUsersFragment : ListFragment
	{
		private int supaPosition;
		private int selectedUserId;
		private bool isDualPane;
		private LocationManager locationManager;
		private String provider;
		private string[] nearbyBios;
		private List<Profile> nearbyUserslist;

		/// <summary>
		/// Gets the location manager and creates a new Geolocation object with
		/// the user's current location and receives a list from our server
		/// containing all of the nearby users based on that geolocation.
		/// The list is parsed and the profile bios are formatted and placed 
		/// into a string array with the same corresponding index of the string
		/// array of nearby usernames.
		/// </summary>
		/// <param name="savedInstanceState">Bundle.</param>
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

            //Create a Geolocation object which handles most of the location related functionality, including server calls
            Geolocation currentLocation = new Geolocation (MainActivity.username, location.Latitude, location.Longitude);

            //Get the list of nearby profiles
            Task<List<Profile>> nearbyProfiles = Task<List<Profile>>.Factory.StartNew(() => 
				{ 
					return currentLocation.GetNearbyUsers().Result;
				});

            //Filter the list of profiles, removing the profile for this user, if it was returned by the server
            nearbyUserslist = new List<Profile>();
			try{
				nearbyUserslist = nearbyProfiles.Result;
				foreach (Profile p in nearbyUserslist){
					if(p.username.Equals(MainActivity.credentials.username)){
						nearbyUserslist.Remove(p);
						break;
					}
				}
			}
			catch(Exception e) {
				string error = e.Message;
				System.Diagnostics.Debug.WriteLine ("\n\nServer offline\n\n" + error);
			}

            //Create the arrays containing the information about the users
			int numUsers = nearbyUserslist.Count;
			string[] nearbyUsers = new string[numUsers];
			nearbyBios = new string[numUsers];

            //Process the information about the nearby users in a formatted string, for display
			for (int i = 0; i < numUsers; i++)
			{
				nearbyUsers [i] = nearbyUserslist [i].username;
				nearbyBios [i] = "⇧ " + nearbyUserslist[i].positive_votes 
					+ "\n⇩ " + nearbyUserslist[i].negative_votes 
					+ "\n\n" + nearbyUserslist [i].gender
					+ "\n\n" + nearbyUserslist [i].bio;
			}
		
            //Set up the adapted to display the list of users
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

		/// <summary>
		/// Saves current instance state.
		/// </summary>
		/// <param name="outState">Bundle.</param>
		public override void OnSaveInstanceState(Bundle outState)
		{  
			base.OnSaveInstanceState(outState);
			outState.PutInt("selected_user_id", selectedUserId);
		}

		/// <summary>
		/// Displays the profile bios of the current username that the user
		/// has clicked on to view.
		/// </summary>
		/// <param name="l">The current list view.</param>
		/// <param name="v">The current view.</param>
		/// <param name="position">The current postion of the list item clicked.</param>
		/// <param name="id">The list id.</param>
		public override void OnListItemClick(ListView l, View v, int position, long id)
		{
			if(supaPosition==position)
			{
				Intent intent2 = new Intent(Context.ApplicationContext,typeof(ViewProfile));
				intent2.PutExtra ("username_from", nearbyUserslist [supaPosition].username);
				StartActivity (intent2);
			}
			ShowProfile(position);
			supaPosition = position;
		}


		/// <summary>
		/// Shows the profile of the current userId that is passed
		/// </summary>
		/// <param name="userId">The index of the selected user</param>
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