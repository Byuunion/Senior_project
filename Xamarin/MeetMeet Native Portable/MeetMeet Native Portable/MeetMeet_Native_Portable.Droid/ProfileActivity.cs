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

namespace MeetMeet_Native_Portable.Droid
{
	public class OnProfileEventArgs: EventArgs {

		public string mUsername;
		public int mPositive_votes;
		public int mNegative_votes;
		public double mCurrent_lat;
		public double mCurrent_long;
		public string mGender;
		public string mBio;


		public string Username{
			get { return mUsername; }
			set { mUsername = value; }
		}

		public int Positive_Votes{
			get { return mPositive_votes; }
			set { mPositive_votes = value; }
		}

		public int Negative_Votes{
			get { return mNegative_votes; }
			set { mNegative_votes = value; }
		}

		public double Current_lat{
			get { return mCurrent_lat; }
			set { mCurrent_lat = value; }
		}

		public double Current_long{
			get { return mCurrent_long; }
			set { mCurrent_long = value; }
		}

		public string Gender{
			get { return mGender; }
			set { mGender = value; }
		}

		public string Bio {
			get { return mBio; }
			set { mBio = value; }
		}

	}


	[Activity (Label = "Profile")]
	public class Profile : DialogFragment{

		private EditText mTxtUsername;
		private EditText mTxtGender;
		private EditText mTxtBio;


	}

}