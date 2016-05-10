using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Locations;


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
		private List<Profile> nearbyUserslist;

		private EditText mMessage;
		private Button mSendMessage;
		public string userSelected;


        /// <summary>
        /// Code to the executed when the activity is created
        /// </summary>
        /// <param name="bundle">Any additional data sent to the activity</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view 
			SetContentView (Resource.Layout.message_spinner);

            //Create the spinner for the name of the nearby users
			Spinner spinner = FindViewById<Spinner> (Resource.Id.spinner);
			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_ItemSelected);

			// Get the location manager
			locationManager = (LocationManager)Config.context.GetSystemService (Context.LocationService);

			// Define the criteria how to select the location provider -> use default
			Criteria criteria = new Criteria ();
			criteria.Accuracy = Accuracy.Fine;
			provider = locationManager.GetBestProvider (criteria, true);
			Location location = locationManager.GetLastKnownLocation (provider);

            //Create a Geolocation object which handles most of the location related functionality, including server calls
			Geolocation currentLocation = new Geolocation (MainActivity.username, location.Latitude, location.Longitude);

            //Get the list of nearby profiles
			Task<List<Profile>> nearbyProfiles = Task<List<Profile>>.Factory.StartNew (() => { 
				return currentLocation.GetNearbyUsers ().Result;
			});

            //Filter the list of profiles, removing the profile for this user, if it was returned by the server
			nearbyUserslist = new List<Profile> ();
			try {
				nearbyUserslist = nearbyProfiles.Result;
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

            //Create an array containing all the usernames of the nearby users
			string[] nearbyUsers = new string[nearbyUserslist.Count];

			for (int i = 0; i < nearbyUserslist.Count; i++) {
				nearbyUsers [i] = nearbyUserslist [i].username;
			}

            //Set up the spinner object to user the array of nearby users
			var adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItemChecked, nearbyUsers);

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
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
        async void MSendMessage_Click (object sender, EventArgs e)
		{
            //Try to send a message to another user
			if (await MessageSender.SendSingleMessage (mMessage.Text, userSelected, MainActivity.credentials, URLs.serverURL + URLs.single_message))
            {

                //Create a message object to store on the device
				Message m = new Message ();
				m.Date = System.DateTime.Now.ToString ();
				m.UserName = userSelected;
				m.MsgText = mMessage.Text;
				m.incoming = false;
				MessageRepository.SaveMessage (m);

                //Clear the input box
				mMessage.Text = "";
				Toast.MakeText (this, "Message Sent!", ToastLength.Short).Show ();
			}
            else
            {
				Toast.MakeText (this, "Message Failed!", ToastLength.Short).Show ();
			}
		}

        /// <summary>
        /// Spinners the item selected.
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
        private void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;

            //Save the name of the user selected for use in sending a message
			userSelected = string.Format ("{0}", spinner.GetItemAtPosition (e.Position));

			string toast = string.Format ("User is {0}", spinner.GetItemAtPosition (e.Position));
			Toast.MakeText (this, toast, ToastLength.Long).Show ();
		}
	}
}
