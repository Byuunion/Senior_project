using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Content;
using Android.Views;

using Newtonsoft.Json;


namespace MeetMeet_Native_Portable.Droid
{
    /// <summary>
    /// Will provide functionality for home page.
    /// </summary>
    [Activity(Label = "Home", Icon = "@drawable/icon")]
    public class HomeActivity : Activity, ILocationListener
    {
        private Profile userProfile;
        private LocationManager locationManager;
        private String provider;

        // Main Menu Items
        private Button mButtonNearbyUsers;
        private Button mButtonUpdateLocation;
        private Button mButtonSendMessage;
        private Button mButtonGroupSendMessage;

        // Show username at top to show Profile was passed correctly
        private TextView mTextViewUsername;

        /// <summary>
        /// Creates the event for the home activity.
        /// Sets layout to home_page, references the Find Nearby Users,
        /// Get Location and Edit Profile button clicks.
        /// </summary>
        /// <param name="bundle">Bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the home_page resource
            SetContentView(Resource.Layout.home_page);

            // Set context to this activity to be passed to NearbyUsersFragment
            Config.context = this;

            // Get profile object from another activity
            var jsonString = Intent.GetStringExtra("UserProfile");
            userProfile = JsonConvert.DeserializeObject<Profile>(jsonString);

            // Set our view from the home_page resource
            SetContentView(Resource.Layout.home_page);

            // References for Home Menu Items
            mButtonNearbyUsers = FindViewById<Button>(Resource.Id.NearbyUsersButton);
            mButtonUpdateLocation = FindViewById<Button>(Resource.Id.SetLocationButton);
            mButtonSendMessage = FindViewById<Button>(Resource.Id.SendMessageButton);
            mButtonGroupSendMessage = FindViewById<Button>(Resource.Id.btnGroupMessage);
            mTextViewUsername = FindViewById<TextView>(Resource.Id.UsernameTextView);


            // Set username text
            mTextViewUsername.Text = userProfile.username;

            //*** Click Events ***
            // Find Nearby Users Click
            mButtonNearbyUsers.Click += delegate
            {
                StartActivity(typeof(NearbyUsersActivity));
            };

            // Set Current Location Click 
            mButtonUpdateLocation.Click += mButtonSetLocation_Click;

            // Send Message Click
            mButtonSendMessage.Click += MButtonSendMsg_Click;

            //Send Group Message Click
            mButtonGroupSendMessage.Click += MButtonGroupSendMessage_Click;

            // Get the location manager
            locationManager = (LocationManager)GetSystemService(Context.LocationService);

            // Define the criteria how to select the location provider
            Criteria criteria = new Criteria();
            criteria.Accuracy = Accuracy.Fine;
            provider = locationManager.GetBestProvider(criteria, true);
        }

        /// <summary>
        /// Starts the group messaging activity when the group messaging button is clicked
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
		void MButtonGroupSendMessage_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(GroupMessage));
            StartActivity(intent);
        }


        /// <summary>
        /// Starts the single messaging activity when the single messaging button is clicked
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
		void MButtonSendMsg_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MessageSpinner));
            StartActivity(intent);
        }

        /// <summary>
        /// Creates the Android options menu located in the upper right hand corner.
        /// </summary>
        /// <param name="menu">Menu.</param>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.home_menu, menu);
            return base.OnPrepareOptionsMenu(menu);
        }

        /// <summary>
        /// Performs the desired action of each menu item when
        /// selected from the drop down menu list.
        /// </summary>
        /// <param name="item">Selected menu item.</param>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Dialog dialog = null;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            //Choose which action to do: Logout, View Messages, Leave Group, Edit Profile, Delete Account
            switch (item.ItemId)
            {
                case Resource.Id.logout:
                    // Create the logout confirmation dialog
                    builder.SetMessage(Resource.String.logout_question)
                        .SetCancelable(false)
                        .SetPositiveButton(Resource.String.yes, (senderAlert, args) =>
                        {

                        //Send the logout request to the server
                        Deleter.DeleteObject(MainActivity.serverURL + MainActivity.login_ext + "/" + MainActivity.credentials.username + "/" + MainActivity.credentials.token);

                            userProfile = null;
                            Intent logoutIntent = new Intent(this, typeof(MainActivity));
                            logoutIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
                            StartActivity(logoutIntent);
                        })
                        .SetNegativeButton(Resource.String.no, (senderAlert, args) => { });

                    //Build and show the logout dialog
                    dialog = builder.Create();
                    dialog.Show();
                    return true;

                case Resource.Id.viewMessages:
                    // Start ViewMessagesActivity 
                    StartActivity(typeof(ViewInbox));
                    return true;

                case Resource.Id.leaveGroup:
                    // Create the leave group confirmation dialog
                    builder.SetMessage(Resource.String.leave_group_question)
                        .SetCancelable(false)
                        .SetPositiveButton(Resource.String.yes, (senderAlert, args) =>
                        {
                            //Tell the server to remove the user from their current group
                            Deleter.DeleteObject(MainActivity.serverURL + MainActivity.user_group + "/" + MainActivity.credentials.username + "/" + MainActivity.credentials.token);
                        })
                        .SetNegativeButton(Resource.String.no, (senderAlert, args) => { });

                    //Build and show the leave group dialog
                    dialog = builder.Create();
                    dialog.Show();
                    return true;

                case Resource.Id.editProfile:
                    // Pass profile object and start EditProfileActivity
                    Intent editIntent = new Intent(this, typeof(EditProfileActivity));
                    var serializedObject = JsonConvert.SerializeObject(userProfile);
                    editIntent.PutExtra("UserProfile", serializedObject);
                    StartActivity(editIntent);
                    return true;

                case Resource.Id.deleteUser:
                    // Create the delete user confirmation dialog
                    builder.SetMessage(Resource.String.delete_question)
                        .SetCancelable(false)
                        .SetPositiveButton(Resource.String.yes, (senderAlert, args) =>
                        {
                            Deleter.DeleteObject(MainActivity.serverURL + "user/" + MainActivity.credentials.username + "/" + MainActivity.credentials.token);
                            userProfile = null;
                            Intent deleteIntent = new Intent(this, typeof(MainActivity));
                            deleteIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
                            StartActivity(deleteIntent);
                        })
                        .SetNegativeButton(Resource.String.no, (senderAlert, args) => { });

                    //Build and show the account deletion dialog
                    dialog = builder.Create();
                    dialog.Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
		/// Request location updates at startup.
		/// </summary>
		protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(provider, 1000, 0, this);
        }

        /// <summary>
        /// Remove the locationlistener updates when Activity is paused.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
        }

        /// <summary>
        /// Constantly updates location
        /// </summary>
        /// <param name="location">The newest location</param>
        public void OnLocationChanged(Location location)
        {
            userProfile.current_lat = location.Latitude;
            userProfile.current_long = location.Longitude;
        }

        /// <summary>
		/// Status of the location services provider.
		/// </summary>
		/// <param name="provider">Type of provider for location services.</param>
		/// <param name="status">Availability of location services.</param>
		/// <param name="extras">Bundle.</param>
		public void OnStatusChanged(String provider, Availability status, Bundle extras)
        {
            // Required for ILocationListener
        }

        /// <summary>
        /// Displays toast to let user know the provider is enabled.
        /// </summary>
        /// <param name="provider">Type of provider for location services.</param>
        public void OnProviderEnabled(String provider)
        {
            Toast.MakeText(this, "Enabled new provider " + provider,
                ToastLength.Short).Show();
        }

        /// <summary>
        /// Displays toast to let user know the provider is disabled.
        /// </summary>
        /// <param name="provider">Type of provider for location services.</param>
        public void OnProviderDisabled(String provider)
        {
            Toast.MakeText(this, "Disabled provider " + provider,
                ToastLength.Short).Show();
        }

        /// <summary>
        /// Click event to set the current location and update our server
        /// with the user's new geolocation information.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void mButtonSetLocation_Click(object sender, EventArgs e)
        {
            Location location = locationManager.GetLastKnownLocation(provider);
            userProfile.current_lat = location.Latitude;
            userProfile.current_long = location.Longitude;
            Geolocation currentLocation = new Geolocation(userProfile.username, location.Latitude, location.Longitude);
            currentLocation.UpdateGeolocation(userProfile.token);
            Toast.MakeText(this, "Set Location Successfully", ToastLength.Short).Show();
        }
    }
}

