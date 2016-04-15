using System;
using System.Collections.Generic;
using Android.Widget;
using Android.App;
using Android;

namespace MeetMeet_Native_Portable.Droid
{
	
	public class MsgListAdapter : BaseAdapter<Message> {
		Activity context = null;
		IList<Message> msgs = new List<Message>();
		//the other text views and the delete button need to be in here
		//utilizing getitemid one can port over that info to generate the write message in the view msg pop up

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
				context.LayoutInflater.Inflate(
					Android.Resource.Layout.SimpleListItem1,
					parent, 
					false)) as TextView;

			//need to add in timestamp 
			view.SetText (item.UserName==""?"<new UserName>":item.UserName, TextView.BufferType.Normal);

			//Finally return the view
			return view;
		}
	}
}
