
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

		public string userNameFrom;
		public static string serverURL = "http://52.91.212.179:8800/";


		async protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.view_profile);

			musernameviewprofile = FindViewById<TextView> (Resource.Id.usernameviewprofile);
			mbioviewprofile = FindViewById<TextView> (Resource.Id.bioviewprofile);
			mpositivevoteviewprofile = FindViewById<TextView> (Resource.Id.positivevoteviewprofile);
			mnegativevoteviewprofile = FindViewById<TextView> (Resource.Id.negativevoteviewprofile);

			// Passed username 

			userNameFrom = Intent.GetStringExtra ("username_from") ?? "Data not available";


			// Get user and bio
			Profile profile = await Getter<Profile>.GetObject(userNameFrom, serverURL + "user/profile/");
			string username = profile.username;
			string bio = profile.bio;
			int posvote = profile.positive_votes;
			int negvote = profile.negative_votes;

			// set username and bio

			musernameviewprofile.Text = username;
			mbioviewprofile.Text = bio;
			mpositivevoteviewprofile.Text = ("" + posvote);
			mnegativevoteviewprofile.Text = ("" + negvote);
		}
	}
}

