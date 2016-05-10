using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;


namespace MeetMeet_Native_Portable.Droid
{
    /// <summary>
    /// Activity to allow the user the view the profile of another user
    /// </summary>
	[Activity (Label = "ViewProfile")]			
	public class ViewProfile : Activity
	{
        //The gui components that we are interacting with
		private TextView musernameviewprofile;
		private TextView mbioviewprofile;
		private TextView mpositivevoteviewprofile;
		private TextView mnegativevoteviewprofile;
		private Button	mUpvote;
		private Button mDownvote;
		private Button mInvite;
		private Button mBlock;
		private Button mUnblock;

        //Information about the user being viewed
		public static string userNameFrom;
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
			mInvite = FindViewById<Button> (Resource.Id.inviteViewProfileButton);
			mBlock = FindViewById<Button> (Resource.Id.blockViewProfileButton);
			mUnblock = FindViewById<Button> (Resource.Id.unblockViewProfileButton);

			// Passed username 
			userNameFrom = Intent.GetStringExtra ("username_from") ?? "Data not available";

			// Get the user's profile information
			profile = await Getter<Profile>.GetObject(URLs.serverURL + URLs.profile + "/" + userNameFrom);
			string username = profile.username;
			string bio = profile.bio;
			int posvote = profile.positive_votes;
			int negvote = profile.negative_votes;

			// set username and bio on the display
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

        /// <summary>
        /// Block the currently viewed user
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
        async void MBlock_Click(object sender, EventArgs e)
        {
            //Try to block the user
            if (await Poster.PostObject(new { username = MainActivity.credentials.username, block_username = profile.username, token = MainActivity.credentials.token },
                   URLs.serverURL + URLs.blacklist_user))
            {
                Toast.MakeText(this, "Successfully blocked user!", ToastLength.Short).Show();
            }
            else {
                Toast.MakeText(this, "Unable to block user", ToastLength.Short).Show();
            }
        }

        /// <summary>
        /// Unblock the currently viewed user
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
		async void MUnblock_Click (object sender, EventArgs e)
		{
            //Try to unblock the user
			if (await Deleter.DeleteObject (URLs.serverURL + URLs.blacklist_user + "/" + MainActivity.credentials.username + "/" + profile.username + "/" + MainActivity.credentials.token)) {
				Toast.MakeText (this, "Successfully unblocked user!", ToastLength.Short).Show ();
			} else {
				Toast.MakeText (this, "Unable to unblock user", ToastLength.Short).Show ();
			}
		}

        /// <summary>
        /// Send an upvote to the currently viewed user
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
		async void MUpvote_Click (object sender, EventArgs e)
		{
            //Try to send an upvote
			if (await Updater.UpdateObject (new {rating_username = MainActivity.credentials.username, token = MainActivity.credentials.token}, 
				URLs.serverURL + URLs.pos_rating + "/" + profile.username)) {
				Toast.MakeText (this, "Successfully rated user!", ToastLength.Short).Show();
			}
			else
				Toast.MakeText (this, "Unable to send positive vote", ToastLength.Short).Show();
		}

        /// <summary>
        /// Send a downvote to the currently viewed user
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
		async void MDownvote_Click (object sender, EventArgs e)
		{
            //Try to send a downvote
			if (await Updater.UpdateObject (new {rating_username = MainActivity.credentials.username, token = MainActivity.credentials.token}, 
				URLs.serverURL + URLs.neg_rating + "/" + profile.username)) {
				Toast.MakeText (this, "Successfully rated user!", ToastLength.Short).Show();
			}
			else
				Toast.MakeText (this, "Unable to send negative vote", ToastLength.Short).Show();
		}

        /// <summary>
        /// Send a group invitation to the currently viewed user
        /// </summary>
        /// <param name="sender">The object that invoked the event</param>
        /// <param name="e">The event arguments</param>
		async void MInvite_Click (object sender, EventArgs e)
		{
            //Try to send the invitation
			if (await MessageSender.SendGroupInvite (profile.username, MainActivity.credentials, URLs.serverURL + URLs.group_invite)) {
				Toast.MakeText (this, "Invite Sent!", ToastLength.Short).Show ();
			} else {

				Toast.MakeText (this, "Invite Failed!", ToastLength.Short).Show ();
			}
		}
	}
}

