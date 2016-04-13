
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

using MeetMeet_Native_Portable;

namespace MeetMeet_Native_Portable.Droid
{
	[Activity (Label = "ViewProfile")]			
	public class ViewProfile : Activity
	{

		private TextView musernameviewprofile;
		private TextView mbioviewprofile;
		private TextView mpositivevoteviewprofile;
		private TextView mnegativevoteviewprofile;
		private Button	mUpvote;
		private Button mDownvote;

		public static string userNameFrom;
		public static string serverURL = "http://52.91.212.179:8800/";
		public static Profile profile;


		async protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.view_profile);

			musernameviewprofile = FindViewById<TextView> (Resource.Id.usernameviewprofile);
			mbioviewprofile = FindViewById<TextView> (Resource.Id.bioviewprofile);
			mpositivevoteviewprofile = FindViewById<TextView> (Resource.Id.positivevoteviewprofile);
			mnegativevoteviewprofile = FindViewById<TextView> (Resource.Id.negativevoteviewprofile);
			mUpvote = FindViewById<Button> (Resource.Id.upvoteButton);
			mDownvote = FindViewById<Button> (Resource.Id.downvoteButton);

			// Passed username 

			userNameFrom = Intent.GetStringExtra ("username_from") ?? "Data not available";


			// Get user and bio
		    profile = await Getter<Profile>.GetObject(userNameFrom, serverURL + "user/profile/");
			string username = profile.username;
			string bio = profile.bio;
			int posvote = profile.positive_votes;
			int negvote = profile.negative_votes;

			// set username and bio

			musernameviewprofile.Text = username;
			mbioviewprofile.Text = bio;
			mpositivevoteviewprofile.Text = ("" + posvote);
			mnegativevoteviewprofile.Text = ("" + negvote);

			mUpvote.Click += MUpvote_Click;
			mDownvote.Click += MDownvote_Click;

		}

		async void MUpvote_Click (object sender, EventArgs e)
		{
			/*profile.positive_votes = profile.positive_votes + 1;
			if (await Updater.UpdateObject (profile, MainActivity.serverURL, MainActivity.profile_ext)) {
			}
			else
				Toast.MakeText (this, "Profile Update Unsuccessful", ToastLength.Short).Show();*/
		}

		async void MDownvote_Click (object sender, EventArgs e)
		{
			/*profile.negative_votes = profile.negative_votes - 1;
			if (await Updater.UpdateObject (profile, MainActivity.serverURL, MainActivity.profile_ext)) {
			}
			else{
				Toast.MakeText (this, "Profile Update Unsuccessful", ToastLength.Short).Show();
			}*/
		}
	}
}

