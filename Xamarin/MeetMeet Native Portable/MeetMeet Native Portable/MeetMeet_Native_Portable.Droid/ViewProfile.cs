
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
		private Button mInvite;
		private Button mBlock;
		private Button mUnblock;

		public static string userNameFrom;
		public static string serverURL = "http://52.91.212.179:8800/";
		public static Profile profile;
		public static Credentials credentials;

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
			mInvite = FindViewById<Button> (Resource.Id.inviteViewProfileButton);
			mBlock = FindViewById<Button> (Resource.Id.blockViewProfileButton);
			mUnblock = FindViewById<Button> (Resource.Id.unblockViewProfileButton);

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

			mInvite.Click += MInvite_Click;
			mBlock.Click += MBlock_Click;
			mUnblock.Click += MUnblock_Click;

		}

		async void MUnblock_Click (object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Trying to block user " + profile.username);
			if (await Deleter.DeleteObject (MainActivity.blacklist_user + "/" + MainActivity.credentials.username + "/" + profile.username + "/" + MainActivity.credentials.token, 
				MainActivity.serverURL )) {
				Toast.MakeText (this, "Successfully unblocked user!", ToastLength.Short).Show ();
			} else {
				Toast.MakeText (this, "Unable to unblock user", ToastLength.Short).Show ();
			}
		}

	
		async void MUpvote_Click (object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Trying to send positive vote to user " + profile.username + " from user " + MainActivity.credentials.username);
			if (await Updater.UpdateObject (new {rating_username = MainActivity.credentials.username, token = MainActivity.credentials.token}, 
				MainActivity.serverURL, MainActivity.pos_rating + "/" + profile.username)) {
				Toast.MakeText (this, "Successfully rated user!", ToastLength.Short).Show();
			}
			else
				Toast.MakeText (this, "Unable to send positive vote", ToastLength.Short).Show();
		}

		async void MDownvote_Click (object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Trying to send negative vote to user " + profile.username + " from user " + MainActivity.credentials.username);
			if (await Updater.UpdateObject (new {rating_username = MainActivity.credentials.username, token = MainActivity.credentials.token}, 
				MainActivity.serverURL, MainActivity.neg_rating + "/" + profile.username)) {
				Toast.MakeText (this, "Successfully rated user!", ToastLength.Short).Show();
			}
			else
				Toast.MakeText (this, "Unable to send negative vote", ToastLength.Short).Show();
		}

		async void MInvite_Click (object sender, EventArgs e)
		{
			if (await MessageSender.SendGroupInvite (profile.username, credentials, serverURL)) {
				Toast.MakeText (this, "Invite Sent!", ToastLength.Short).Show ();
			}
		}

		async void MBlock_Click (object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Trying to block user " + profile.username);
			if (await Poster.PostObject (new {username = MainActivity.credentials.username, block_username = profile.username, token = MainActivity.credentials.token}, 
				   MainActivity.serverURL + MainActivity.blacklist_user)) {
				Toast.MakeText (this, "Successfully blocked user!", ToastLength.Short).Show ();
			} else {
				Toast.MakeText (this, "Unable to block user", ToastLength.Short).Show ();
			}
		}

	}
}

