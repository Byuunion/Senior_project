using System;
using System.Collections.Generic;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.App;
using Android;
using Newtonsoft.Json;

namespace MeetMeet_Native_Portable.Droid
{
	
	public class MsgListAdapter : BaseAdapter<Message> {
		Activity context = null;
		IList<Message> msgs = new List<Message>();
		//the other text views and the delete button need to be in here
		//utilizing getitemid one can port over that info to generate the write message in the view msg pop up
		private ImageButton mButtonDeleteMsg;
		private TextView mTextViewUsername;
		private TextView mTextViewDate;

		public MsgListAdapter (Activity context, IList<Message> msgs) : base ()
		{
			this.context = context;
			this.msgs = msgs;
		}

		public override Message this[int position]
		{
			get { return msgs[position]; }
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override int Count
		{
			get { return msgs.Count; }
		}


//		protected override void OnCreate(Bundle bundle)
//		{
//			base.OnCreate (bundle);
//
//			// Set our view from the home_page resource
//			SetContentView (Resource.Layout.msg_adapter);
//			mButtonDeleteMsg = FindViewById<ImageButton> (Resource.Id.delButton);
//			mTextViewUsername = FindViewById<TextView> (Resource.Id.UserNameView);
//			mTextViewDate = FindViewById<TextView> (Resource.Id.dateView);
//			var item = msgs [position];
//
//			// Get Message object from other activities
//			var jsonString = Intent.GetStringExtra("UserProfile");
//			userProfile = JsonConvert.DeserializeObject<Profile>(jsonString);
//
//
//		}

		//delete button -> final long id = getItemId(position);

		//this was the default method xamarin uses for list view.
		//this needs to be changed because my view needs to display username and timestamp
		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			// Get our object for position
			var item = msgs[position];			

			//Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
			// gives us some performance gains by not always inflating a new view
			// will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()
			var view = (convertView ??
			           context.LayoutInflater.Inflate (
				           Resource.Layout.msg_adapter,
				           parent, 
					false)) as ListView; 
										//TextView;
			//view.SetContentView (Resource.Layout.msg_adapter);
			mButtonDeleteMsg = view.FindViewById<ImageButton> (Resource.Id.delButton);
			mTextViewUsername = view.FindViewById<TextView> (Resource.Id.UserNameView);
			mTextViewDate = view.FindViewById<TextView> (Resource.Id.dateView);

			mTextViewUsername.Text = item.UserName;
			mTextViewDate.Text = item.Date;

			mButtonDeleteMsg.Click += async (object sender, EventArgs e) => {
				msgs.Remove(item);
			};


			//Finally return the view
			return view;
		}

	}
}
