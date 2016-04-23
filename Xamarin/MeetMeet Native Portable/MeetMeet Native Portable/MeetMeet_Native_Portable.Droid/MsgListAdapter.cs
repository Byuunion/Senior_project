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
	
    /// <summary>
    /// Adapter for user messages
    /// </summary>
	public class MsgListAdapter : BaseAdapter<Message> {
		ViewInbox context = null;
		IList<Message> msgs = new List<Message>();


		//the other text views and the delete button need to be in here
		//utilizing getitemid one can port over that info to generate the write message in the view msg pop up
		private ImageButton mButtonDeleteMsg;
		//private TextView mTextViewUsername;
		private TextView mTextViewDate;
        //private TextView mTextIncoming;
        private TextView mTextViewMessage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msgs"></param>
		public MsgListAdapter (ViewInbox context, IList<Message> msgs) : base ()
		{
			this.context = context;
			this.msgs = msgs;
			System.Diagnostics.Debug.WriteLine (msgs.Count);
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
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			// Get our object for position
			var item = msgs[position];			

			// Inflate convertView from our item layout
			var view = context.LayoutInflater.Inflate (Resource.Layout.msg_adapter, parent, false); 
			view.Clickable = true;

			mButtonDeleteMsg = view.FindViewById<ImageButton> (Resource.Id.delButton);
			//mTextViewUsername = view.FindViewById<TextView> (Resource.Id.UserNameView);
			mTextViewDate = view.FindViewById<TextView> (Resource.Id.dateView);
            //mTextIncoming = view.FindViewById<TextView> (Resource.Id.textView2);
            mTextViewMessage = view.FindViewById<TextView>(Resource.Id.messageText);


            mButtonDeleteMsg.Focusable = false;
			mButtonDeleteMsg.FocusableInTouchMode = false;
			mButtonDeleteMsg.Clickable = true;

			//mTextViewUsername.Text = item.UserName;
			mTextViewDate.Text = item.Date;
            //mTextIncoming.Text = (item.incoming ? "Message from: " : "Message to: ");
            mTextViewMessage.Text = item.MsgText;

            if (item.incoming)
            {
                mTextViewMessage.SetTextColor(Android.Graphics.Color.Cyan);
            }
            else
            {
                mTextViewMessage.SetTextColor(Android.Graphics.Color.Green);
            }

            view.Click += (object sender, EventArgs e) => {
				context.viewMessage(position);
			};

            //Delete button clicked
			mButtonDeleteMsg.Click += (object sender, EventArgs e) => {
				MessageRepository.DeleteMessage(item);
				Toast.MakeText(context, "message with: " + item.UserName + " deleted", ToastLength.Short).Show();
				msgs.Remove(item);
				context.removeMessage(position);
			};



			//Finally return the view
			return view;
		}

	}
}
