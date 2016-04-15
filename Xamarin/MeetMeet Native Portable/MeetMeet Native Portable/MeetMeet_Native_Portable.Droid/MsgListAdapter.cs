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

			view.SetText (item.UserName==""?"<new UserName>":item.UserName, TextView.BufferType.Normal);

			//Finally return the view
			return view;
		}
	}
}
