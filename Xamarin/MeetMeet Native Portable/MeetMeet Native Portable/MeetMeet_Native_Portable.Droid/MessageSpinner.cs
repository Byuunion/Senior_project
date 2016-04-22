
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Support.V4.App;
using Android.Locations;
using System.Threading.Tasks;




namespace MeetMeet_Native_Portable.Droid
{
	[Activity (Label = "MessageSpinner")]

	/// <summary>
	/// Message spinner. This is the activity for sending single messages to 
	/// users that a currently signed on and nearby. This activity uses the 
	/// message_spinner layout. 
	/// </summary>
	public class MessageSpinner : Activity
	{

		private LocationManager locationManager;
		private String provider;
		private string[] nearbyBios;
		private List<Profile> nearbyUserslist;

		private EditText mMessage;
		private Button mSendMessage;
		public static string serverURL = "http://52.91.212.179:8800/";
		public string userSelected;




		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view 
			SetContentView (Resource.Layout.message_spinner);


			Spinner spinner = FindViewById<Spinner> (Resource.Id.spinner);

			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_ItemSelected);

			/////////////////////////////////////// This should be refactored 


			// Get the location manager
			locationManager = (LocationManager)Config.context.GetSystemService (Context.LocationService);
			// Define the criteria how to select the location provider -> use default
			Criteria criteria = new Criteria ();
			criteria.Accuracy = Accuracy.Fine;
			provider = locationManager.GetBestProvider (criteria, true);
			Location location = locationManager.GetLastKnownLocation (provider);
			Geolocation currentLocation = new Geolocation (MainActivity.username, location.Latitude, location.Longitude);
			//Geolocation currentLocation = new Geolocation (MainActivity.username, 39.77689537, -75.11926562);

			Task<List<Profile>> task3 = Task<List<Profile>>.Factory.StartNew (() => { 
				return currentLocation.GetNearbyProfiles ().Result;
			});

			nearbyUserslist = new List<Profile> ();
			try {
				nearbyUserslist = task3.Result;
				foreach (Profile p in nearbyUserslist) {
					if (p.username.Equals (MainActivity.credentials.username)) {
						nearbyUserslist.Remove (p);
						break;
					}
				}
			} catch (Exception e) {
				string error = e.Message;
				System.Diagnostics.Debug.WriteLine ("\n\nServer offline\n\n" + error);
			}


			int numUsers = nearbyUserslist.Count;
			string[] nearbyUsers = new string[numUsers];
			nearbyBios = new string[numUsers];

			for (int i = 0; i < numUsers; i++) {
				nearbyUsers [i] = nearbyUserslist [i].username;
				nearbyBios [i] = "⇧ " + nearbyUserslist [i].positive_votes
				+ "\n⇩ " + nearbyUserslist [i].negative_votes
				+ "\n\n" + nearbyUserslist [i].gender
				+ "\n\n" + nearbyUserslist [i].bio;
			}


			var adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItemChecked, nearbyUsers);

			///////////////////////////////////////////////////////////////

			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;

			mMessage = FindViewById<EditText> (Resource.Id.sendMessageTxt);
			mSendMessage = FindViewById<Button> (Resource.Id.btnSendMsg);


			mSendMessage.Click += MSendMessage_Click;
		}

		/// <summary>
		/// On send message click, this will retrieve the data inputed by the user
		/// and attempt to send a message via MessageSender. User will be notified
		/// if the message was sent successfully or not.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void MSendMessage_Click (object sender, EventArgs e)
		{
			if (await MessageSender.SendSingleMessage (mMessage.Text, userSelected, MainActivity.credentials, serverURL + MainActivity.single_message)) {
				Message m = new Message ();
				m.Date = System.DateTime.Now.ToString ();
				m.UserName = userSelected;
				m.MsgText = mMessage.Text;
				m.incoming = false;
				MessageRepository.SaveMessage (m);
				mMessage.Text = "";
				Toast.MakeText (this, "Message Sent!", ToastLength.Short).Show ();
			} else {
				Toast.MakeText (this, "Message Failed!", ToastLength.Short).Show ();
			}
		}

		/// <summary>
		/// Spinners the item selected.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;

			userSelected = string.Format ("{0}", spinner.GetItemAtPosition (e.Position));

			string toast = string.Format ("User is {0}", spinner.GetItemAtPosition (e.Position));
			Toast.MakeText (this, toast, ToastLength.Long).Show ();
		}
	}
}
